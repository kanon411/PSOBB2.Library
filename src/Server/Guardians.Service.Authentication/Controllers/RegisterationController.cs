using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[Route("api/registeration")]
	public class RegisterationController : Controller
	{
		private UserManager<GuardiansApplicationUser> UserManager { get; }

		private ILogger<RegisterationController> Logger { get; }

		/// <inheritdoc />
		public RegisterationController(UserManager<GuardiansApplicationUser> userManager, ILogger<RegisterationController> logger)
		{
			if(userManager == null) throw new ArgumentNullException(nameof(userManager));
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			UserManager = userManager;
			Logger = logger;
		}

#warning Dont ever deploy this for real
		[HttpPost]
		public async Task<IActionResult> RegisterDev([FromQuery] string username, [FromQuery] string password)
		{
			if(string.IsNullOrWhiteSpace(username))
				return BadRequest("Invalid username");

			if(string.IsNullOrWhiteSpace(password))
				return BadRequest("Invalid password.");

			await UserManager.CreateAsync(new GuardiansApplicationUser()
			{
				UserName = username,
				Email = "dev@dev.com"
			}, password);

			return Ok();
		}
	}
}
