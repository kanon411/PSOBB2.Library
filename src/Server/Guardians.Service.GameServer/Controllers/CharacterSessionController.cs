using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		//TODO: This can't be unit tested because all the logic is written in an SQL stored procedure.
		/// <summary>
		/// Endpoint that the ZoneServers should query for attempted session claiming.
		/// ONLY zoneserver roles should be able to call this. NEVER allow clients to call this endpoint.
		/// </summary>
		/// <returns></returns>
		[AuthorizeJwt(GuardianApplicationRole.ZoneServer)]
		[HttpPost("claim")]
		public async Task<IActionResult> TryClaimSession([FromBody] ZoneServerTryClaimSessionRequest request)
		{
			if(!this.ModelState.IsValid)
				return BadRequest(); //TODO: Send JSON back too.

			//We do not use the actual requesting ZoneServer's JWT id.
			//We must use the user id they're trying to claim a session for.
			string guid = ClaimsReader.GetGloballyUniqueUserId(User);

			//TODO: Load the zone id.
			
			//TODO: Verify that the zone id is correct. Right now we aren't providing it and the query doesn't enforce it.
			//We don't validate characterid/accountid association manually. It is implemented in the tryclaim SQL instead.
			//It additionally also checks the zone relation for the session so it will fail if it's invalid for the provided zone.
			//Therefore we don't need to make 3/4 database calls/queries to claim a session. Just one stored procedure call.
			//This is preferable. A result code will be used to indicate the exact error in the future. For now it just fails if it fails.
			bool sessionClaimed = await CharacterSessionRepository.TryClaimUnclaimedSession(request.PlayerAccountId, request.CharacterId);

			return Ok(new ZoneServerTryClaimSessionResponse(sessionClaimed ? ZoneServerTryClaimSessionResponseCode.Success : ZoneServerTryClaimSessionResponseCode.GeneralServerError)); //TODO
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
			if(!await CharacterSessionRepository.TryCreateAsync(new CharacterSessionModel(characterId, 1)))
				return new CharacterSessionEnterResponse(CharacterSessionEnterResponseCode.GeneralServerError);
			
			//TODO: Better zone handling
			return new CharacterSessionEnterResponse(1);
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
