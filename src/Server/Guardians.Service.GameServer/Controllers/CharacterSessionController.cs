using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[Route("api/[controller]")]
	public sealed class CharacterSessionController : AuthorizationReadyController
	{
		private ICharacterRepository CharacterRepository { get; }

		private ICharacterSessionRepository CharacterSessionRepository { get; }

		/// <inheritdoc />
		public CharacterSessionController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger, [FromServices] ICharacterRepository characterRepository, [FromServices] ICharacterSessionRepository characterSessionRepository) 
			: base(claimsReader, logger)
		{
			CharacterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
			CharacterSessionRepository = characterSessionRepository ?? throw new ArgumentNullException(nameof(characterSessionRepository));
		}

		[AuthorizeJwt]
		[HttpGet("testclaim")]
		public async Task<IActionResult> Test()
		{
			bool b = await CharacterSessionRepository.TryClaimUnclaimedSession(ClaimsReader.GetUserIdInt(User), 3);

			return Ok($"Result: {b}");
		}

		[HttpPost("enter/{id}")]
		[NoResponseCache]
		[AuthorizeJwt]
		public async Task<CharacterSessionEnterResponse> EnterSession([FromRoute(Name = "id")] int characterId)
		{
			if(!await IsCharacterIdValidForUser(characterId, CharacterRepository))
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.InvalidCharacterIdError);

			int accountId = ClaimsReader.GetUserIdInt(User);

			//This checks to see if the account, not just the character, has an active session.
			//We do this before we check anything to reject quick even though the query behind this
			//may be abit more expensive
			if(await CharacterSessionRepository.AccountHasActiveSession(accountId))
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.AccountAlreadyHasCharacterSession);

			//They may have a session entry already, which is ok. So long as they don't have an active claimed session
			//which the above query checks for.
			bool hasSession = await CharacterSessionRepository.ContainsAsync(characterId);

			//We need to check active or not
			if(hasSession)
			{
				//If it's active we can just retrieve the data and send them off on their way
				CharacterSessionModel sessionModel = await CharacterSessionRepository.RetrieveAsync(characterId);

				//TODO: Handle case when we have an inactive session that can be claimed
				return new CharacterSessionEnterResponse(sessionModel.ZoneId);
			}

			//If we've made it this far we'll need to create a session (because one does not exist) for the character
			//but we need player location data first (if they've never entered the world they won't have any
			//TODO: Handle location loading
			//TODO: Handle deafult
			if(!await CharacterSessionRepository.TryCreateAsync(new CharacterSessionModel(characterId, 0)))
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.GeneralServerError);
			
			//TODO: Better zone handling
			return new CharacterSessionEnterResponse(0);
		}

		/// <summary>
		/// Indicates if the provided character id is valid for the user in the message context.
		/// </summary>
		/// <param name="characterId">The id to check.</param>
		/// <param name="characterRepository">The character repository service.</param>
		/// <returns>True if the character id is valid/</returns>
		private async Task<bool> IsCharacterIdValidForUser(int characterId, ICharacterRepository characterRepository)
		{
			//We only support positive character ids so if they request a less than 0 it's invalid and likely spoofed
			//or if they request an id they don't own
			//or if it's an not a known character
			return characterId >= 0 &&
				await characterRepository.ContainsAsync(characterId) && 
				(await characterRepository.RetrieveAsync(characterId)).AccountId == ClaimsReader.GetUserIdInt(User);
		}
	}
}
