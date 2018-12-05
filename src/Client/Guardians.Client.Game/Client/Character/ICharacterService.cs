using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Guardians.Client.Common.Attributes;
using Refit;

namespace Guardians
{
	/// <summary>
	/// Proxy interface for character service requests.
	/// </summary>
	[Headers("User-Agent: GuardiansClient")]
	public interface ICharacterService
	{
		/// <summary>
		/// Requests a list of all available characters
		/// using the provided <see cref="authToken"/> to
		/// authorize the request.
		/// </summary>
		/// <param name="authToken">The authentication token.</param>
		/// <returns>The character request.</returns>
		[Headers("Cache-Control: max-age=60")]
		[Get("/api/characters")]
		Task<CharacterListResponse> GetCharacters([AuthenticationToken] string authToken);

		/// <summary>
		/// TODO Doc
		/// </summary>
		/// <param name="characterId"></param>
		/// <returns></returns>
		[Headers("Cache-Control: max-age=360")]
		[Get("/api/characters/name/{id}")]
		Task<NameQueryResponse> NameQuery([AliasAs("id")] int characterId);

		/// <summary>
		/// TODO DOC
		/// </summary>
		/// <param name="characterId"></param>
		/// <param name="authToken"></param>
		/// <returns></returns>
		[Post("/api/charactersession/enter/{id}")]
		Task<CharacterSessionEnterResponse> TryEnterSession([AliasAs("id")] int characterId, [AuthenticationToken] string authToken);

		/// <summary>
		/// Gets a character's session id, if authorized.
		/// </summary>
		/// <param name="characterId">The character id to get session data for.</param>
		/// <param name="authToken">The authentication token for authorization.</param>
		/// <returns>The session data response.</returns>
		[Get("/{id}/data")]
		[Headers("Cache-Control: NoCache")] //TODO: I frgot what this is suppose to be
		Task<CharacterSessionDataResponse> GetCharacterSessionData([AliasAs("id")] int characterId, [AuthenticationToken] string authToken);
	}
}
