using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using GladNet;
using Reinterpret.Net;

namespace GladMMO
{
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class SessionAuthChallengeEventHandler : BaseGameClientGameMessageHandler<SessionAuthChallengeEvent>
	{
		/// <inheritdoc />
		public SessionAuthChallengeEventHandler(ILog logger) 
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GamePacketPayload> context, SessionAuthChallengeEvent payload)
		{
			Logger.Info($"Recieved {payload.GetOperationCode()} from TrinityCore.");
			Logger.Info($"Sending CMSG_AUTH_SESSION to TrinityCore.");

			//HelloKitty: So, auth challenge data is useless unless you wanna reconnect/redirect
			//so we should just sent the auth session request
			//TODO: We have a hack here to use realmID as the account id. We really need to move eventually to reading the auth token on TC.
			await context.PayloadSendService.SendMessage(new SessionAuthProofRequest(ClientBuild.Wotlk_3_2_2a, "ADMIN", ((int)0).Reinterpret(), new RealmIdentification(1), new byte[20], new AddonChecksumInfo[0]));
		}
	}
}
