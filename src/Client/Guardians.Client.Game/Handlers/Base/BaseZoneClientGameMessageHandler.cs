using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// Base handler for all game handlers.
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	public abstract class BaseZoneClientGameMessageHandler<TSpecificPayloadType> : IPeerPayloadSpecificMessageHandler<TSpecificPayloadType, GameClientPacketPayload>
		where TSpecificPayloadType : GameServerPacketPayload
	{
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected BaseZoneClientGameMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public abstract Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, TSpecificPayloadType payload);
	}
}
