using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;
using GladNet;
using Nito.AsyncEx;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ICharacterSelectionEntryDataChangeEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionOnStartGameserverAuthenticateInitializable : IGameInitializable, ICharacterSelectionEntryDataChangeEventSubscribable
	{
		private ILog Logger { get; }

		/// <inheritdoc />
		public event EventHandler<CharacterSelectionEntryDataChangeEventArgs> OnCharacterSelectionEntryChanged;

		private ICharacterService CharacterServiceQueryable { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

		/// <inheritdoc />
		public CharacterSelectionOnStartGameserverAuthenticateInitializable([NotNull] ILog logger,
			[NotNull] ICharacterService characterServiceQueryable,
			[NotNull] IReadonlyAuthTokenRepository authTokenRepository)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterServiceQueryable = characterServiceQueryable ?? throw new ArgumentNullException(nameof(characterServiceQueryable));
			AuthTokenRepository = authTokenRepository ?? throw new ArgumentNullException(nameof(authTokenRepository));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			UnityAsyncHelper.UnityMainThreadContext.PostAsync(async () =>
			{
				CharacterListResponse listResponse = await CharacterServiceQueryable.GetCharacters(AuthTokenRepository.RetrieveWithType())
					.ConfigureAwait(false);

				//TODO: Handle errors
				foreach(var character in listResponse.CharacterIds)
				{
					var entityGuid = new NetworkEntityGuidBuilder()
						.WithId(character)
						.WithType(EntityType.Player)
						.Build();

					OnCharacterSelectionEntryChanged?.Invoke(this, new CharacterSelectionEntryDataChangeEventArgs(entityGuid));
				}
			});

			return;
		}
	}
}
