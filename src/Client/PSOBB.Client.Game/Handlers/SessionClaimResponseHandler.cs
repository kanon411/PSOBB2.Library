using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace GladMMO
{
	/// <summary>
	/// The handling of the session claim response.
	/// </summary>
	[SceneTypeCreate(GameSceneType.PreZoneBurstingScreen)]
	public sealed class SessionClaimResponseHandler : BaseZoneClientGameMessageHandler<ClientSessionClaimResponsePayload>
	{
		/// <inheritdoc />
		public SessionClaimResponseHandler(ILog logger)
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, ClientSessionClaimResponsePayload payload)
		{
			//TODO: Actually handle this. Right now it's just demo code, it actually could fail.
			if(Logger.IsInfoEnabled)
				Logger.Info($"Session Claim Response: {payload.ResultCode}");

			//This is not when/where we spawn the local player, the server will tell us when to actually spawn.

			//if we fail we'd probably want to return to the titlescreen, not easy to recover from this failure.
			return Task.CompletedTask;
		}
	}
}
