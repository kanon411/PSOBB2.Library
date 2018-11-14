using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	//TODO: This is just demo code, we won't want to handle starting the game like this.
	//TODO: This can't be a game initializable because those are invoked when the session is claimed
	[Injectee]
	public sealed class ZoneClientCharacterSessionInitializable : MonoBehaviour
	{
		[Inject]
		private IPeerPayloadSendService<GameClientPacketPayload> PayloadSender { get; }

		[Inject]
		private ICharacterDataRepository CharacterRepository { get; }

		/// <inheritdoc />
		public ZoneClientCharacterSessionInitializable(IPeerPayloadSendService<GameClientPacketPayload> payloadSender, ICharacterDataRepository characterRepository)
		{
			PayloadSender = payloadSender;
			CharacterRepository = characterRepository;
		}

		/// <inheritdoc />
		public async Task Start()
		{
			await Task.Delay(1500);

			await PayloadSender.SendMessage(new ClientSessionClaimRequestPayload("TODO", CharacterRepository.CharacterId));
		}
	}
}
