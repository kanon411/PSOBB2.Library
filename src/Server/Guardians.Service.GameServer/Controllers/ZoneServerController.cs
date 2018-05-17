using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guardians
{
	[Route("api/[controller]")]
	public class ZoneServerController : Controller
	{
		private IZoneServerRepository ZoneRepository { get; }

		/// <inheritdoc />
		public ZoneServerController([FromServices] IZoneServerRepository zoneRepository)
		{
			ZoneRepository = zoneRepository ?? throw new ArgumentNullException(nameof(zoneRepository));
		}

		/*[ProducesJson]
		[HttpPost("register")]
		[NoResponseCache]
		[AuthorizeJwt(GuardianApplicationRole.ZoneServer)]
		public async Task<IActionResult> RegisterZoneServer()
		{
			//TODO: JSON
			if(!ModelState.IsValid)
				return BadRequest();

			//Check if it's already registered
			//ZoneRepository.

			//TODO
			return null;
		}*/

		//We don't require authorization because it's not unique per-player
		//It's also not a secret. They could auth then grab every endpoint.
		//No reason to try to hide it.
		[HttpGet("{id}/endpoint")]
		[ProducesJson]
		[ResponseCache]
		public async Task<IActionResult> GetServerEndpoint([FromQuery(Name = "id")] int zoneId)
		{
			if(!ModelState.IsValid)
				return BadRequest(new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.GeneralRequestError));

			//We reuse the service discovery response model
			if(!await ZoneRepository.ContainsAsync(zoneId))
				return BadRequest(new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.ServiceUnlisted));

			//Small interval for race condition. So we try catch.
			try
			{
				ZoneInstanceEntryModel zone = await ZoneRepository.RetrieveAsync(zoneId);

				//Should be good, we just send them the endpoint
				if(zone != null)
					return Ok(new ResolveServiceEndpointResponse(new ResolvedEndpoint(zone.ZoneServerAddress, zone.ZoneServerPort)));
			}
			catch(Exception)
			{
				//TODO: Logging/event
			}

			return BadRequest(new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.GeneralRequestError));
		}
	}
}
