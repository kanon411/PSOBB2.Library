using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace GladMMO
{
	/// <summary>
	/// This is the default handler that is invoked when an unknown payload is encountered.
	/// Or a payload is encountered that doesn't have a registered handler.
	/// </summary>
	public sealed class ZoneClientDefaultRequestHandler : IPeerPayloadSpecificMessageHandler<GameServerPacketPayload, GameClientPacketPayload>
	{
		private ILog Logger { get; }

		/// <inheritdoc />
		public ZoneClientDefaultRequestHandler([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, GameServerPacketPayload payload)
		{
			if(Logger.IsWarnEnabled)
				Logger.Warn($"Recieved unhandable Payload: {payload.GetType().Name}");

			return Task.CompletedTask;
		}
	}
}
