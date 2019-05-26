using System;
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
	[Route("api/namequery")]
	public class NameQueryController : AuthorizationReadyController
	{
		//TODO: Eventually we will support more than players.
		private ICharacterRepository CharacterRepository { get; }

		/// <inheritdoc />
		public NameQueryController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger, 
			ICharacterRepository characterRepository) 
			: base(claimsReader, logger)
		{
			if(characterRepository == null) throw new ArgumentNullException(nameof(characterRepository));

			CharacterRepository = characterRepository;
		}

		//TODO: Conslidate/centralize name query stuff via entity GUID.
		[AllowAnonymous]
		[ProducesJson]
		[ResponseCache(Duration = 360)] //We want to cache this for a long time. But it's possible with name changes that we want to not cache forever
		[HttpGet]
		public async Task<IActionResult> NameQuery([NotNull] [FromBody] NetworkEntityGuid entityGuid)
		{
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			if(entityGuid.EntityId < 0)
				return BuildNotFoundUnknownIdResponse();

			bool knownId = await CharacterRepository.ContainsAsync(entityGuid.EntityId);

			//TODO: JSON Response
			if(!knownId)
				return BuildNotFoundUnknownIdResponse();

			//Else if it is a known id we should grab the name of the character
			string name = await CharacterRepository.RetrieveNameAsync(entityGuid.EntityId);

			return Ok(new NameQueryResponse(name));
		}

		private IActionResult BuildNotFoundUnknownIdResponse()
		{
			return NotFound(new NameQueryResponse(NameQueryResponseCode.UnknownIdError));
		}
	}
}
