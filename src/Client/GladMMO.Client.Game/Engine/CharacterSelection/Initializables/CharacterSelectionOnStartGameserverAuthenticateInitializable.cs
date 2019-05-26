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

		private IEntityNameQueryable EntityNameQueryable { get; }

		/// <inheritdoc />
		public CharacterSelectionOnStartGameserverAuthenticateInitializable([NotNull] ILog logger,
			[NotNull] ICharacterService characterServiceQueryable,
			[NotNull] IReadonlyAuthTokenRepository authTokenRepository,
			[NotNull] IEntityNameQueryable entityNameQueryable)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterServiceQueryable = characterServiceQueryable ?? throw new ArgumentNullException(nameof(characterServiceQueryable));
			AuthTokenRepository = authTokenRepository ?? throw new ArgumentNullException(nameof(authTokenRepository));
			EntityNameQueryable = entityNameQueryable ?? throw new ArgumentNullException(nameof(entityNameQueryable));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			UnityAsyncHelper.UnityMainThreadContext.PostAsync(async () =>
			{
				try
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

						//Do a namequery so it's in the cache for when anything tries to get entities name.
						await EntityNameQueryable.RetrieveAsync(entityGuid)
							.ConfigureAwait(false);

						OnCharacterSelectionEntryChanged?.Invoke(this, new CharacterSelectionEntryDataChangeEventArgs(entityGuid));
					}
				}
				catch(Exception e)
				{
					Logger.Error($"Encountered Error: {e.Message}");
					throw;
				}
			});

			return;
		}
	}
}
