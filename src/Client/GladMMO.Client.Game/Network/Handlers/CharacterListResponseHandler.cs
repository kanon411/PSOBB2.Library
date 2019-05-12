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
		/// <inheritdoc />
		public CharacterListResponseHandler(ILog logger) 
			: base(logger)
		{

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
				OnCharacterSelectionEntryChanged?.Invoke(this, new CharacterSelectionEntryDataChangeEventArgs(character.Data.CharacterGuid));
			}
		}
	}
}