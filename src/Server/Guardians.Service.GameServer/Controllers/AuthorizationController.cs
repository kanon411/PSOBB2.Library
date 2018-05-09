using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardians
{
	[Route("api/[controller]")]
	public class AuthorizationController : Controller
	{
		
		private IClaimsPrincipalReader ClaimsReader { get; }

		/// <inheritdoc />
		public AuthorizationController(IClaimsPrincipalReader claimsReader)
		{
			if(claimsReader == null) throw new ArgumentNullException(nameof(claimsReader));
			ClaimsReader = claimsReader;
		}

		[Authorize]
		[HttpGet("check")]
		public IActionResult Check()
		{
			if(ModelState.IsValid && this.User.Identity.IsAuthenticated)
				return Ok();
			else
				return Unauthorized();
		}
	}
}
