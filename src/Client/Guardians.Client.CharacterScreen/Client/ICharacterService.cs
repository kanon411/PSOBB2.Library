using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TypeSafe.Http.Net;

namespace Guardians
{
	/// <summary>
	/// Proxy interface for character service requests.
	/// </summary>
	[Header("User-Agent", "GuardiansClient")]
	public interface ICharacterService
	{
		/// <summary>
		/// Requests a list of all available characters
		/// using the provided <see cref="authToken"/> to
		/// authorize the request.
		/// </summary>
		/// <param name="authToken">The authentication token.</param>
		/// <returns>The character request.</returns>
		[Header("Cache-Control", "max-age=60")]
		[Get("api/characters")]
		Task<CharacterListResponse> GetCharacters([AuthenticationToken] string authToken);

		/// <summary>
		/// TODO Doc
		/// </summary>
		/// <param name="characterId"></param>
		/// <returns></returns>
		[Header("Cache-Control", "max-age=360")]
		[Get("api/name/{id}")]
		Task<CharacterNameQueryResponse> NameQuery([AliasAs("id")] int characterId);
	}
}
