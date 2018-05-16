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
		//We don't require authorization because it's not unique per-player
		//It's also not a secret. They could auth then grab every endpoint.
		//No reason to try to hide it.
		[HttpGet("{id}/endpoint")]
		[ProducesJson]
		[ResponseCache]
		public async Task<IActionResult> GetServerEndpoint([FromQuery(Name = "id")] int zoneId)
		{
			//TODO: JSON Response
			if(!ModelState.IsValid)
				return BadRequest();

			//TODO:
			return null;
		}
	}
}
