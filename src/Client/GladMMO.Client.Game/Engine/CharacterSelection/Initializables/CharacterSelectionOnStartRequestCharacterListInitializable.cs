using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ICharacterSelectionEntryDataChangeEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionOnStartRequestCharacterListInitializable : IGameInitializable, ICharacterSelectionEntryDataChangeEventSubscribable
	{
		//private ICharacterService CharacterQueryable { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public event EventHandler<CharacterSelectionEntryDataChangeEventArgs> OnCharacterSelectionEntryChanged;

		/// <inheritdoc />
		public CharacterSelectionOnStartRequestCharacterListInitializable([NotNull] IReadonlyAuthTokenRepository authTokenRepository, [NotNull] ILog logger)
		{
			//CharacterQueryable = characterQueryable ?? throw new ArgumentNullException(nameof(characterQueryable));
			AuthTokenRepository = authTokenRepository ?? throw new ArgumentNullException(nameof(authTokenRepository));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		//TODO: Race condition here because it's possible the subsciber hasn't subscribed just yet.
		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			/*CharacterListResponse listResponse = await CharacterQueryable.GetCharacters(AuthTokenRepository.RetrieveWithType())
				.ConfigureAwait(true);

			if(!listResponse.isSuccessful || listResponse.CharacterIds.Count == 0)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to query character list. Recieved ResultCode: {listResponse.ResultCode}");

				//We don't have character creation... Soooo, nothing we can do right now.
				return;
			}

			//TODO: Should we make this API spit out network guids?
			foreach(var characterId in listResponse.CharacterIds)
			{
				NetworkEntityGuid entityGuid = new NetworkEntityGuidBuilder()
					.WithType(EntityType.Player)
					.WithId(characterId)
					.Build();

				OnCharacterSelectionEntryChanged?.Invoke(this, new CharacterSelectionEntryDataChangeEventArgs(entityGuid));
			}*/
		}
	}
}
