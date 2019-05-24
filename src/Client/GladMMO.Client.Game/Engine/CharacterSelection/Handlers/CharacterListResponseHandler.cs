using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;
using GladMMO;
using GladNet;

namespace FreecraftCore.Swarm
{
	[AdditionalRegisterationAs(typeof(ICharacterSelectionEntryDataChangeEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterListResponseHandler : BaseGameClientGameMessageHandler<CharacterListResponse>, ICharacterSelectionEntryDataChangeEventSubscribable
	{
		private INameQueryStorageable NameQueryStorage { get; }

		/// <inheritdoc />
		public CharacterListResponseHandler(ILog logger, [NotNull] INameQueryStorageable nameQueryStorage) 
			: base(logger)
		{
			NameQueryStorage = nameQueryStorage ?? throw new ArgumentNullException(nameof(nameQueryStorage));
		}

		/// <inheritdoc />
		public event EventHandler<CharacterSelectionEntryDataChangeEventArgs> OnCharacterSelectionEntryChanged;

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GamePacketPayload> context, CharacterListResponse payload)
		{
			foreach(var c in payload.Characters)
				Logger.Info($"Recieved Character: {c.Data.CharacterName}");

			if(!payload.isValid || payload.Characters.Length == 0)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to query character list. Recieved ResultCode: {"recieved"}");

				//We don't have character creation... Soooo, nothing we can do right now.
				return;
			}

			//TODO: Should we make this API spit out network guids?
			foreach(var character in payload.Characters)
			{
				//We don't need to do namequery for these now, and other things can expect it exists immediately.
				NameQueryStorage.Add(character.Data.CharacterGuid, character.Data.CharacterName);
				OnCharacterSelectionEntryChanged?.Invoke(this, new CharacterSelectionEntryDataChangeEventArgs(character.Data.CharacterGuid));
			}
		}
	}
}