using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[Route("api/characters")]
	public class CharacterController : AuthorizationReadyController
	{
		private ICharacterRepository CharacterRepository { get; }

		/// <inheritdoc />
		public CharacterController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger, 
			ICharacterRepository characterRepository) 
			: base(claimsReader, logger)
		{
			if(characterRepository == null) throw new ArgumentNullException(nameof(characterRepository));
			CharacterRepository = characterRepository;
		}
		
		[ResponseCache(Duration = 10)] //Jagex crumbled for a day due to name checks. So, we should cache for 10 seconds. Probably won't change much.
		[AllowAnonymous]
		[HttpGet("name/validate")]
		public async Task<IActionResult> ValidateCharacterName([FromQuery] string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			bool containsName = await ValidateNameAvailability(name);

			//TODO: Handle JSON model response.
			return Ok($"Result: {containsName}");
		}

		private async Task<bool> ValidateNameAvailability(string name)
		{
			//TODO: Add a dependency that can filter and check the validate the name's format/characters/length

			//Now we have to check if a character exists with this name
			return !await CharacterRepository.ContainsAsync(name);
		}

		//TODO: Support recieve creation model JSON. Same with response.
		[AuthorizeJwt] //is it IMPORTANT that this method authorize the user. Don't know the accountid otherwise even, would be impossible.
		[HttpPost("create/{name}")]
		[NoResponseCache]
		public async Task<IActionResult> CreateCharacter([FromRoute] string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			int accountId = ClaimsReader.GetUserIdInt(User);

			bool nameIsAvailable = await ValidateNameAvailability(name);

			//TODO: JSON response
			if(!nameIsAvailable)
				return BadRequest();

			//Otherwise we should try to create. There is a race condition here that can cause it to still fail
			//since others could create a character with this name before we finish after checking
			bool result = await CharacterRepository.TryCreateAsync(new CharacterDatabaseModel(accountId, name));

			//TODO: JSON
			return Ok($"Result: {result} {name}:{accountId} (accountid)");
		}
	}
}
