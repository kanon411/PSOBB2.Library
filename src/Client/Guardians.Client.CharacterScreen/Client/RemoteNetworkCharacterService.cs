using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TypeSafe.Http.Net;

namespace Guardians
{
	public sealed class RemoteNetworkCharacterService : ICharacterService
	{
		private ICharacterService CharacterService { get; }

		public RemoteNetworkCharacterService(string baseUrl)
		{
			if(string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

			CharacterService = TypeSafeHttpBuilder<ICharacterService>
				.Create()
				.RegisterJsonNetSerializer()
				.RegisterDefaultSerializers()
				.RegisterDotNetHttpClient(baseUrl, new FiddlerEnabledWebProxyHandler())
				.Build();
		}

		public RemoteNetworkCharacterService(Task<string> baseUrl)
		{
			CharacterService = TypeSafeHttpBuilder<ICharacterService>
				.Create()
				.RegisterJsonNetSerializer()
				.RegisterDefaultSerializers()
				.RegisterDotNetHttpClient(baseUrl, new FiddlerEnabledWebProxyHandler())
				.Build();
		}

		/// <inheritdoc />
		public Task<CharacterListResponse> GetCharacters(string authToken)
		{
			return CharacterService.GetCharacters(authToken);
		}

		/// <inheritdoc />
		public Task<CharacterNameQueryResponse> NameQuery(int characterId)
		{
			return CharacterService.NameQuery(characterId);
		}

		/// <inheritdoc />
		public Task<CharacterSessionEnterResponse> TryEnterSession(int characterId, string authToken)
		{
			return CharacterService.TryEnterSession(characterId, authToken);
		}
	}
}
