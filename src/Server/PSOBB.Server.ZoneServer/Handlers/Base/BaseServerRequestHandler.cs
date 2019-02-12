using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace PSOBB
{
	public abstract class BaseServerRequestHandler<TSpecificPayloadType> : IPeerPayloadSpecificMessageHandler<TSpecificPayloadType, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>
		where TSpecificPayloadType : GameClientPacketPayload
	{
		protected Common.Logging.ILog Logger { get; }

		/// <inheritdoc />
		protected BaseServerRequestHandler([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public abstract Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload);
	}
}
