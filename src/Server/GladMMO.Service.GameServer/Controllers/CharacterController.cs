﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace GladMMO
{
	[Route("api/characters")]
	public class CharacterController : AuthorizationReadyController
	{
		//TODO: Check and enforce character limit
		//TODO: Add logging to these controllers
		private ICharacterRepository CharacterRepository { get; }

		/// <inheritdoc />
		public CharacterController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger, 
			ICharacterRepository characterRepository) 
			: base(claimsReader, logger)
		{
			if(characterRepository == null) throw new ArgumentNullException(nameof(characterRepository));

			CharacterRepository = characterRepository;
		}

		[ProducesJson]
		[ResponseCache(Duration = 10)] //Jagex crumbled for a day due to name checks. So, we should cache for 10 seconds. Probably won't change much.
		[AllowAnonymous]
		[HttpGet("name/validate")]
		public async Task<IActionResult> ValidateCharacterName([FromQuery] string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			//TODO: Finer grain picking apart. We want to indicate the failure reason.
			bool nameIsAvailable = await ValidateNameAvailability(name);

			return Ok(new CharacterNameValidationResponse(nameIsAvailable ? CharacterNameValidationResponseCode.Success : CharacterNameValidationResponseCode.NameIsUnavailable));
		}

		private async Task<bool> ValidateNameAvailability(string name)
		{
			//TODO: Add a dependency that can filter and check the validate the name's format/characters/length

			//Now we have to check if a character exists with this name
			return !await CharacterRepository.ContainsAsync(name);
		}

		//TODO: Support recieve creation model JSON. Same with response.
		[ProducesJson]
		[AuthorizeJwt] //is it IMPORTANT that this method authorize the user. Don't know the accountid otherwise even, would be impossible.
		[HttpPost("create/{name}")]
		[NoResponseCache]
		public async Task<IActionResult> CreateCharacter([FromRoute] string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			int accountId = ClaimsReader.GetUserIdInt(User);

			bool nameIsAvailable = await ValidateNameAvailability(name);

			if(!nameIsAvailable)
				return BadRequest(new CharacterCreationResponse(CharacterCreationResponseCode.NameUnavailableError));

			//TODO: Don't expose the database table model
			//Otherwise we should try to create. There is a race condition here that can cause it to still fail
			//since others could create a character with this name before we finish after checking
			bool result = await CharacterRepository.TryCreateAsync(new CharacterEntryModel(accountId, name));

			//TODO: JSON
			return Created("TODO", new CharacterCreationResponse(CharacterCreationResponseCode.Success));
		}

		//TODO: Conslidate/centralize name query stuff via entity GUID.
		[AllowAnonymous]
		[ProducesJson]
		[ResponseCache(Duration = 360)] //We want to cache this for a long time. But it's possible with name changes that we want to not cache forever
		[HttpGet("name/{id}")]
		public async Task<IActionResult> NameQuery([FromRoute(Name = "id")] int characterId)
		{
			if(characterId < 0)
				return BuildNotFoundUnknownIdResponse();

			bool knownId = await CharacterRepository.ContainsAsync(characterId);

			//TODO: JSON Response
			if(!knownId)
				return BuildNotFoundUnknownIdResponse();

			//Else if it is a known id we should grab the name of the character
			string name = await CharacterRepository.RetrieveNameAsync(characterId);

			return Ok(new NameQueryResponse(name));
		}

		[HttpGet]
		[AuthorizeJwt]
		[ProducesJson]
		public async Task<CharacterListResponse> GetCharacters()
		{
			int accountId = ClaimsReader.GetUserIdInt(User);

			//So to check characters we just need to query for the
			//characters with this account id
			int[] characterIds = await CharacterRepository.CharacterIdsForAccountId(accountId);

#warning This is just for the test build, we need to change this
			ProjectVersionStage.AssertInternalTesting();
			if(characterIds.Length == 0)
			{
				//We just create a new one for testing.
				bool result = await CharacterRepository.TryCreateAsync(new CharacterEntryModel(accountId, ClaimsReader.GetUserName(User)))
					.ConfigureAwait(false);

				if(result)
					//Just return the get, a character should now exist.
					return await GetCharacters();
				else
					return new CharacterListResponse(CharacterListResponseCode.NoCharactersFoundError);
			}
			/*if(characterIds.Length == 0)
				return new CharacterListResponse(CharacterListResponseCode.NoCharactersFoundError);*/
			
			//The reason we only provide the IDs is all other character data can be looked up
			//by the client when it needs it. Like name query, visible/character details/look stuff.
			//No reason to send all this data when they may only need names. Which can be queried through the known API
			return new CharacterListResponse(characterIds);
		}

		//There is a reason we have the zone send the mapid instead of looking up the world/map id
		//based on the zone that is requesting this location be set. That's because a zone may be linked in some way
		//to another map/zone. So it may set the location of the character to another map (example it may walk through a dungeon instance portal)
		//and to provide the ability for the zone to set the new position it should have in the new map we use mapid.
		[AllowAnonymous]
		[HttpPost("location")]
		public async Task<IActionResult> UpdateCharacterLocation(
			[FromBody] ZoneServerCharacterLocationSaveRequest saveRequest,
			[NotNull] [FromServices] ICharacterLocationRepository locationRepository)
		{
			if(locationRepository == null) throw new ArgumentNullException(nameof(locationRepository));

			int characterId = saveRequest.CharacterId;

			if(characterId <= 0 || !await CharacterRepository.ContainsAsync(characterId)
				.ConfigureAwait(false))
				return NotFound();

			//TODO: Is this the best way to deal with this?
			if(await locationRepository.ContainsAsync(characterId).ConfigureAwait(false))
				await locationRepository.UpdateAsync(characterId, BuildCharacterLocationFromSave(characterId, saveRequest))
					.ConfigureAwait(false);
			else
				await locationRepository.TryCreateAsync(BuildCharacterLocationFromSave(characterId, saveRequest))
					.ConfigureAwait(false);

			return Ok();
		}

		private static CharacterLocationModel BuildCharacterLocationFromSave(int characterId, ZoneServerCharacterLocationSaveRequest saveRequest)
		{
			return new CharacterLocationModel(characterId, saveRequest.Position.x, saveRequest.Position.y, saveRequest.Position.z, saveRequest.MapId);
		}

		[ProducesJson]
		[HttpGet("location/{id}")]
		[NoResponseCache]
		//TODO: Renable ZoneServer authorization eventually.
		//[AuthorizeJwt(GuardianApplicationRole.ZoneServer)]
		public async Task<IActionResult> GetCharacterLocation([FromRoute(Name = "id")] int characterId, [NotNull] [FromServices] ICharacterLocationRepository locationRepository)
		{
			if(locationRepository == null) throw new ArgumentNullException(nameof(locationRepository));

			if(characterId <= 0 || !await CharacterRepository.ContainsAsync(characterId)
				.ConfigureAwait(false))
				return Json(new ZoneServerCharacterLocationResponse(ZoneServerCharacterLocationResponseCode.CharacterDoesntExist));

			//So, the character exists and we now need to check if we can find a location for it. It may not have one, for whatever reason.
			//so we need to handle the case where it has none (maybe new character, or was manaully wiped).

			if(!await locationRepository.ContainsAsync(characterId).ConfigureAwait(false))
				return Json(new ZoneServerCharacterLocationResponse(ZoneServerCharacterLocationResponseCode.NoLocationDefined));

			//Otherwise, let's load and send the result
			CharacterLocationModel locationModel = await locationRepository.RetrieveAsync(characterId)
				.ConfigureAwait(false);

			//TODO: Integrate Map Id design into Schema, and implement it here.
			return Json(new ZoneServerCharacterLocationResponse(new Vector3(locationModel.XPosition, locationModel.YPosition, locationModel.ZPosition), 1));
		}

		private IActionResult BuildNotFoundUnknownIdResponse()
		{
			return NotFound(new NameQueryResponse(NameQueryResponseCode.UnknownIdError));
		}
	}
}
