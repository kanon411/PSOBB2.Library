using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using GladNet;

namespace GladMMO
{
	/// <summary>
	/// Base handler for all game handlers.
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	public abstract class BaseGameClientGameMessageHandler<TSpecificPayloadType> : IPeerMessageHandler<GamePacketPayload, GamePacketPayload>
		where TSpecificPayloadType : GamePacketPayload
	{
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected BaseGameClientGameMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		//TODO: Add exception logging support
		/// <inheritdoc />
		public abstract Task HandleMessage(IPeerMessageContext<GamePacketPayload> context, TSpecificPayloadType payload);

		/// <inheritdoc />
		public bool CanHandle(NetworkIncomingMessage<GamePacketPayload> message)
		{
			return message.Payload is TSpecificPayloadType;
		}

		/// <inheritdoc />
		public async Task<bool> TryHandleMessage(IPeerMessageContext<GamePacketPayload> context, NetworkIncomingMessage<GamePacketPayload> message)
		{
			if(!CanHandle(message))
				return false;

			await HandleMessage(context, (TSpecificPayloadType)message.Payload)
				.ConfigureAwait(false);

			return true;
		}
	}
}