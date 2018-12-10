using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using Nito.AsyncEx;

namespace Guardians
{
	/// <summary>
	/// Simplfied version of <see cref="ControlledEntityRequestHandler{TSpecificPayloadType, TLockingPolicyType, TLockingContextType}"/> that
	/// works on locking over an entity globally (read) and grabs the context
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	public abstract class ControlledEntityRequestHandler<TSpecificPayloadType> : ControlledEntityRequestHandler<TSpecificPayloadType, GlobalEntityResourceLockingPolicy, NetworkEntityGuid> where TSpecificPayloadType : GameClientPacketPayload
	{
		/// <inheritdoc />
		protected ControlledEntityRequestHandler(ILog logger, IReadonlyConnectionEntityCollection connectionIdToEntityMap, GlobalEntityResourceLockingPolicy lockingPolicy) 
			: base(logger, connectionIdToEntityMap, lockingPolicy)
		{

		}

		/// <inheritdoc />
		protected sealed override NetworkEntityGuid GenerateLockContext(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload)
		{
			//We just grab the entity, and assume that they want this.
			return ExtractEntityGuidFromContext(context);
		}
	}

	/// <summary>
	/// Base <see cref="IPeerPayloadSpecificMessageHandler{TPayloadType,TOutgoingPayloadType}"/> handler for
	/// messages that require a controlled/associated <see cref="NetworkEntityGuid"/> with the session to be handled.
	/// For example, movement. Can't handle movement packets if the session doesn't even have an associated entity.
	/// </summary>
	/// <typeparam name="TSpecificPayloadType"></typeparam>
	/// <typeparam name="TLockingPolicyType"></typeparam>
	public abstract class ControlledEntityRequestHandler<TSpecificPayloadType, TLockingPolicyType, TLockingContextType> : BaseServerRequestHandler<TSpecificPayloadType>
		where TSpecificPayloadType : GameClientPacketPayload
		where TLockingPolicyType : class, IContextualResourceLockingPolicy<TLockingContextType>
	{
		private IReadonlyConnectionEntityCollection ConnectionIdToEntityMap { get; }

		private TLockingPolicyType LockingPolicy { get; }

		//TODO: Don't use dictionary, creatre interface.
		protected ControlledEntityRequestHandler(
			[NotNull] ILog logger, 
			[NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap,
			[NotNull] TLockingPolicyType lockingPolicy)
			: base(logger)
		{
			ConnectionIdToEntityMap = connectionIdToEntityMap ?? throw new ArgumentNullException(nameof(connectionIdToEntityMap));
			LockingPolicy = lockingPolicy ?? throw new ArgumentNullException(nameof(lockingPolicy));
		}

		public override async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload)
		{
			//TODO: What do we do in cases where the entity 
			ProjectVersionStage.AssertBeta();
			//TODO: We may want a timeout to prevent production deadlocks
			//Just need reader locking to lock on the policy specified by the implementer.
			using(var lockObj = await LockingPolicy.ReaderLockAsync(GenerateLockContext(context, payload), CancellationToken.None)
				.ConfigureAwait(false))
			{
				//We need to check this, if we recieve a message that requires a controlled entity then we should not handle this message
				//and log this. It's possible it was spoofed or something. Or there is an error somewhere in logic.
				if(!ConnectionIdToEntityMap.ContainsKey(context.Details.ConnectionId))
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Recieved: {payload.GetType().Name} from Connection: {context.Details.ConnectionId} but no entity guid associated.");

					return;
				}

				//We just dispatch to child handler, who will use the payload, context and guid.
				await HandleMessage(context, payload, ExtractEntityGuidFromContext(context))
					.ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Generates the context required by the specified <typeparamref name="TLockingContextType"/>.
		/// </summary>
		/// <returns>The context to use for locking.</returns>
		protected abstract TLockingContextType GenerateLockContext(IPeerSessionMessageContext<GameServerPacketPayload> context, TSpecificPayloadType payload);

		protected NetworkEntityGuid ExtractEntityGuidFromContext(IPeerSessionMessageContext<GameServerPacketPayload> context)
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
