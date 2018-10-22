using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	/// <summary>
	/// Base <see cref="IPeerPayloadSpecificMessageHandler{TPayloadType,TOutgoingPayloadType}"/> handler for
	/// messages that require a controlled/associated <see cref="NetworkEntityGuid"/> with the session to be handled.
	/// For example, movement. Can't handle movement packets if the session doesn't even have an associated entity.
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	public abstract class ControlledEntityRequestHandler<TSpecificPayloadType> : IPeerPayloadSpecificMessageHandler<TSpecificPayloadType, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>> 
		where TSpecificPayloadType : GameClientPacketPayload
	{
		private IReadonlyConnectionEntityCollection ConnectionIdToEntityMap { get; }

		protected Common.Logging.ILog Logger { get; }

		//TODO: Don't use dictionary, creatre interface.
		protected ControlledEntityRequestHandler([NotNull] ILog logger, [NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap)
		{
			ConnectionIdToEntityMap = connectionIdToEntityMap ?? throw new ArgumentNullException(nameof(connectionIdToEntityMap));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload)
		{
			//We need to check this, if we recieve a message that requires a controlled entity then we should not handle this message
			//and log this. It's possible it was spoofed or something. Or there is an error somewhere in logic.
			if(!ConnectionIdToEntityMap.ContainsKey(context.Details.ConnectionId))
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Recieved: {payload.GetType().Name} from Connection: {context.Details.ConnectionId} but no entity guid associated.");

				return Task.CompletedTask;
			}

			//We just dispatch to child handler, who will use the payload, context and guid.
			return HandleMessage(context, payload, ConnectionIdToEntityMap[context.Details.ConnectionId]);
		}

		//TODO: Should we create a new context instead?
		protected abstract Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload, NetworkEntityGuid guid);
	}
}
