using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace GladMMO
{
	//TODO: This is just demo code, we won't want to handle starting the game like this.
	//TODO: This can't be a game initializable because those are invoked when the session is claimed
	[Injectee]
	public sealed class ZoneClientCharacterSessionInitializable : MonoBehaviour
	{
		[Inject]
		private IPeerPayloadSendService<GameClientPacketPayload> PayloadSender { get; set; }

		[Inject]
		private ICharacterDataRepository CharacterRepository { get; set; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; set; }

		/// <inheritdoc />
		public ZoneClientCharacterSessionInitializable(IPeerPayloadSendService<GameClientPacketPayload> payloadSender, ICharacterDataRepository characterRepository)
		{
			PayloadSender = payloadSender;
			CharacterRepository = characterRepository;
		}

		/// <inheritdoc />
		public async Task Start()
		{
			ProjectVersionStage.AssertAlpha();
			//TODO: We need this? Can we add support to GladNet3 to queue up unconnected messages?
			//await Task.Delay(1500);

			//TODO: We're sending with Bearer but there is nothing validating both sides expect that.
			await PayloadSender.SendMessage(new ClientSessionClaimRequestPayload(AuthTokenRepo.RetrieveWithType(), CharacterRepository.CharacterId));
		}
	}
}
