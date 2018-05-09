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
		
		[ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)] //Jagex crumbled for a day due to name checks. So, we should cache for 10 seconds. Probably won't change much.
		[AllowAnonymous]
		[HttpGet("name/validate")]
		public async Task<IActionResult> ValidateCharacterName([FromQuery] string name)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			//TODO: Add a dependency that can filter and check the validate the name's format/characters/length

			//Now we have to check if a character exists with this name
			bool containsName = await CharacterRepository.ContainsAsync(name);

			//TODO: Handle JSON model response.
			return Ok($"Result: {containsName}");
		}
	}
}
