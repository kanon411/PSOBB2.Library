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
	public abstract class ControlledEntityRequestHandler<TSpecificPayloadType> : BaseServerRequestHandler<TSpecificPayloadType>
		where TSpecificPayloadType : GameClientPacketPayload
	{
		private IReadonlyConnectionEntityCollection ConnectionIdToEntityMap { get; }

		//TODO: Don't use dictionary, creatre interface.
		protected ControlledEntityRequestHandler([NotNull] ILog logger, [NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap)
			: base(logger)
		{
			ConnectionIdToEntityMap = connectionIdToEntityMap ?? throw new ArgumentNullException(nameof(connectionIdToEntityMap));
		}

		public override Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload)
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
			return HandleMessage(context, payload, ExtractEntityGuidFromContext(context));
		}

		private NetworkEntityGuid ExtractEntityGuidFromContext(IPeerSessionMessageContext<GameServerPacketPayload> context)
		{
			return ConnectionIdToEntityMap[context.Details.ConnectionId];
		}

		//TODO: Should we create a new context instead?
		protected abstract Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload, NetworkEntityGuid guid);

		/// <summary>
		/// Retrieves the mapped object of type TObjectType from the provided <see cref="map"/>.
		/// </summary>
		/// <typeparam name="TObjectType">The object map type.</typeparam>
		/// <param name="map">The map to retrieve from.</param>
		/// <param name="context">The context used to get the entity.</param>
		/// <returns></returns>
		protected TObjectType GetEntityMappedObject<TObjectType>(IReadonlyEntityGuidMappable<TObjectType> map, IPeerSessionMessageContext<GameServerPacketPayload> context)
		{
			return GetEntityMappedObject(map, context.Details.ConnectionId);
		}

		/// <summary>
		/// Retrieves the mapped object of type TObjectType from the provided <see cref="map"/>.
		/// </summary>
		/// <typeparam name="TObjectType">The object map type.</typeparam>
		/// <param name="map">The map to retrieve from.</param>
		/// <param name="connectionId">The context used to get the entity.</param>
		/// <returns></returns>
		protected TObjectType GetEntityMappedObject<TObjectType>(IReadonlyEntityGuidMappable<TObjectType> map, int connectionId)
		{
			NetworkEntityGuid entityGuid = ConnectionIdToEntityMap[connectionId];
			if(!map.ContainsKey(entityGuid))
			{
				throw new InvalidOperationException($"Entity: {entityGuid} did not have a registered service for Type: {typeof(TObjectType).Name}");
			}

			//TODO: This is a race condition here
			ProjectVersionStage.AssertAlpha();
			return map[entityGuid];
		}
	}
}
