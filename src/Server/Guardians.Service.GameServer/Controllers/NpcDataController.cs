using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[Route("api/npcdata")]
	public class NpcDataController : AuthorizationReadyController
	{
		/// <inheritdoc />
		public NpcDataController(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadyController> logger) 
			: base(claimsReader, logger)
		{

		}

		//TODO: Conslidate/centralize name query stuff via entity GUID.
		[AllowAnonymous]
		[ProducesJson]
		[ResponseCache(Duration = int.MaxValue)] //NPC names can be cached forever.
		[HttpGet("name/{id}")]
		public async Task<IActionResult> NameQuery([FromRoute(Name = "id")] int npcId, [FromServices] INpcTemplateRepository templateRepository)
		{
			if(templateRepository == null) throw new ArgumentNullException(nameof(templateRepository));

			if(npcId < 0)
				return BuildNotFoundUnknownIdResponse();

			bool knownId = await templateRepository.ContainsAsync(npcId);

			//TODO: JSON Response
			if(!knownId)
				return BuildNotFoundUnknownIdResponse();

			//Else if it is a known id we should grab the name of the character
			string name = await templateRepository.RetrieveNameAsync(npcId);

			return Ok(new NameQueryResponse(name));
		}

		private IActionResult BuildNotFoundUnknownIdResponse()
		{
			return NotFound(new NameQueryResponse(NameQueryResponseCode.UnknownIdError));
		}
	}
}
