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
	public sealed class ZoneClientDefaultRequestHandler : BaseZoneClientGameMessageHandler<GameServerPacketPayload>
	{
		/// <inheritdoc />
		public ZoneClientDefaultRequestHandler(ILog logger)
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, GameServerPacketPayload payload)
		{
			if(Logger.IsWarnEnabled)
				Logger.Warn($"Recieved unhandable Payload: {payload.GetType().Name}");

			return Task.CompletedTask;
		}
	}
}
