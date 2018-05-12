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

		[HttpPost("enter/{id}")]
		[NoResponseCache]
		[AuthorizeJwt]
		public async Task<CharacterSessionEnterResponse> EnterSession([FromRoute(Name = "id")] int characterId)
		{
			if(!await IsCharacterIdValidForUser(characterId, CharacterRepository))
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.InvalidCharacterIdError);

			int accountId = ClaimsReader.GetUserIdInt(User);

			//If the character id is actually valid then we need to check for existing sessions
			//if a session exists we should allow them to resume ONLY if the session is inactive
			//if no session exists then we should create one based on information on their saved location
			//it is possible their saved location was in a dead non-static zone (like an instance server)
			//so we'll need to throw them back into the static world if that is the case
			bool hasSession = await CharacterSessionRepository.ContainsAsync(characterId);

			//We need to check active or not
			if(hasSession)
			{
				CharacterSessionModel sessionModel = await CharacterSessionRepository.RetrieveAsync(characterId);

				if(sessionModel.IsSessionActive)
					return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.CharacterSessionAlreadyActiveError);

				//TODO: Handle case when we have an inactive session that can be claimed
				return new CharacterSessionEnterResponse(sessionModel.ZoneId);
			}

			//TODO: Fix race condition related to multiple in-progress session claims from seperate characters that exists on seperate zones
			//Even if no session exists it's possible they've started a session on the account on another character
			bool accountHasActiveSession = await CharacterSessionRepository.AccountHasActiveSession(accountId);

			if(accountHasActiveSession)
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.AccountAlreadyHasCharacterSession);

			//If we've made it this far we can create a session (because one does not exist) for the character
			//but we need player location data first (if they've never entered the world they won't have any
			//TODO: Handle location loading
			//TODO: Handle deafult
			if(!await CharacterSessionRepository.TryCreateAsync(new CharacterSessionModel(characterId, 0, false)))
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
			return User.Identity.IsAuthenticated && //this should always be true 
				characterId >= 0 &&
				await characterRepository.ContainsAsync(characterId) && 
				(await characterRepository.RetrieveAsync(characterId)).AccountId == ClaimsReader.GetUserIdInt(User);
		}
	}
}
