using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Guardians
{
	public sealed class AsyncEndpointCharacterService : BaseAsyncEndpointService<ICharacterService>, ICharacterService
	{
		/// <inheritdoc />
		public AsyncEndpointCharacterService(Task<string> futureEndpoint) 
			: base(futureEndpoint)
		{
		}

		/// <inheritdoc />
		public AsyncEndpointCharacterService(Task<string> futureEndpoint, RefitSettings settings) 
			: base(futureEndpoint, settings)
		{

		}

		/// <inheritdoc />
		public async Task<CharacterListResponse> GetCharacters(string authToken)
		{
			return await (await GetService().ConfigureAwait(false)).GetCharacters(authToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<NameQueryResponse> NameQuery(int characterId)
		{
			return await (await GetService().ConfigureAwait(false)).NameQuery(characterId).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<CharacterSessionEnterResponse> TryEnterSession(int characterId, string authToken)
		{
			return await (await GetService().ConfigureAwait(false)).TryEnterSession(characterId, authToken);
		}
	}
}
