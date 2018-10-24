using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// This is the default handler that is invoked when an unknown payload is encountered.
	/// Or a payload is encountered that doesn't have a registered handler.
	/// </summary>
	public sealed class ZoneClientDefaultRequestHandler : IPeerPayloadSpecificMessageHandler<GameClientPacketPayload, GameServerPacketPayload>
	{
		private ILog Logger { get; }

		/// <inheritdoc />
		public ZoneClientDefaultRequestHandler(ILog logger)
		{
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;
		}

		/// <inheritdoc />
		public Task HandleMessage(IPeerMessageContext<GameServerPacketPayload> context, GameClientPacketPayload payload)
		{
			if(Logger.IsWarnEnabled)
				Logger.Warn($"Recieved unhandable Payload: {payload.GetType().Name}");

			return Task.CompletedTask;
		}
	}
}
