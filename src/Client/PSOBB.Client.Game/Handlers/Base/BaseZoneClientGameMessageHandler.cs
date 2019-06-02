using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	/// <summary>
	/// Base handler for all game handlers.
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	public abstract class BaseZoneClientGameMessageHandler<TSpecificPayloadType> : IPeerMessageHandler<GameServerPacketPayload, GameClientPacketPayload>
		where TSpecificPayloadType : GameServerPacketPayload
	{
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected BaseZoneClientGameMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		//TODO: Add exception logging support
		/// <inheritdoc />
		public abstract Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, TSpecificPayloadType payload);

		/// <inheritdoc />
		public bool CanHandle(NetworkIncomingMessage<GameServerPacketPayload> message)
		{
			return message.Payload is TSpecificPayloadType;
		}

		/// <inheritdoc />
		public async Task<bool> TryHandleMessage(IPeerMessageContext<GameClientPacketPayload> context, NetworkIncomingMessage<GameServerPacketPayload> message)
		{
			if(!CanHandle(message))
				return false;

			await HandleMessage(context, (TSpecificPayloadType)message.Payload)
				.ConfigureAwait(false);

			return true;
		}
	}
}
