using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace GladMMO
{
	public sealed class RemoteNetworkedNameQueryService : INameQueryService
	{
		private ICharacterService CharacterService { get; }

		/// <inheritdoc />
		public RemoteNetworkedNameQueryService(ICharacterService characterService)
		{
			CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
		}

		/// <inheritdoc />
		public string Retrieve(int id)
		{
			return RetrieveAsync(id)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}

		/// <inheritdoc />
		public async Task<string> RetrieveAsync(int id)
		{
			NameQueryResponse queryResponse = await CharacterService.NameQuery(id)
				.ConfigureAwait(false);

			if(!queryResponse.isSuccessful)
				throw new KeyNotFoundException($"Failed to retrieve Key: {id} from {nameof(INameQueryService)}. Error: {queryResponse.ResultCode}");

			return queryResponse.EntityName;
		}
	}
}
