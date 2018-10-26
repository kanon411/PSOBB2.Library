using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// The handling of the session claim response.
	/// </summary>
	public sealed class SessionClaimResponseHandler : IPeerPayloadSpecificMessageHandler<ClientSessionClaimResponsePayload, GameClientPacketPayload>
	{
		private ILog Logger { get; }

		/// <inheritdoc />
		public SessionClaimResponseHandler(ILog logger)
		{
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;
		}

		/// <inheritdoc />
		public Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, ClientSessionClaimResponsePayload payload)
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
