using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	//TODO: Refactor clean this mess up
	[GameInitializableOrdering(2)]
	[CollectionsLocking(LockType.Write)]
	public sealed class PlayerEntityEntryManager : IGameTickable, ITickableSkippable
	{
		private IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>> PlayerEntitySessionDequeable { get; }

		private IFactoryCreatable<GameObject, PlayerEntityCreationContext> PlayerFactory { get; }

		private INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> SpawnPayloadSender { get; }

		private ILog Logger { get; }

		private IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> EntityDataUpdateFactory { get; }

		private ISessionCollection SessionCollection { get; }

		private PlayerSessionDeconstructionQueue SessionCleanupQueue { get; }

		//If it's empty, this can be skipped.
		/// <inheritdoc />
		public bool IsTickableSkippable => PlayerEntitySessionDequeable.isEmpty;

		/// <inheritdoc />
		public PlayerEntityEntryManager(
			[NotNull] IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>> playerEntitySessionDequeable, 
			[NotNull] IFactoryCreatable<GameObject, PlayerEntityCreationContext> playerFactory,
			[NotNull] INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> spawnPayloadSender,
			[NotNull] ILog logger,
			[NotNull] IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> entityDataUpdateFactory,
			[NotNull] ISessionCollection sessionCollection,
			[NotNull] PlayerSessionDeconstructionQueue sessionCleanupQueue)
		{
			PlayerEntitySessionDequeable = playerEntitySessionDequeable ?? throw new ArgumentNullException(nameof(playerEntitySessionDequeable));
			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
			SpawnPayloadSender = spawnPayloadSender ?? throw new ArgumentNullException(nameof(spawnPayloadSender));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			EntityDataUpdateFactory = entityDataUpdateFactory ?? throw new ArgumentNullException(nameof(entityDataUpdateFactory));
			SessionCollection = sessionCollection ?? throw new ArgumentNullException(nameof(sessionCollection));
			SessionCleanupQueue = sessionCleanupQueue ?? throw new ArgumentNullException(nameof(sessionCleanupQueue));
		}

		/// <inheritdoc />
		public void Tick()
		{
			//If we have no new players we can just 
			if(PlayerEntitySessionDequeable.isEmpty)
				return;

			//TODO: Should we limit this? We might want to stagger this or else under extreme conditions we could lag the main thread.
			while(!PlayerEntitySessionDequeable.isEmpty)
			{
				KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext> dequeuedPlayerSession = PlayerEntitySessionDequeable.Dequeue();

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Dequeueing entity creation request for: {dequeuedPlayerSession.Key.EntityType}:{dequeuedPlayerSession.Key.EntityId}");

				//TODO: This is test data
				EntityFieldDataCollection<EntityDataFieldType> testData = new EntityFieldDataCollection<EntityDataFieldType>();

				//TODO: Test values set
				testData.SetFieldValue(EntityDataFieldType.EntityCurrentHealth, 100);
				testData.SetFieldValue(EntityDataFieldType.EntityMaxHealth, 120);

				//TODO: Time stamp
				//TODO: We should check if the result is valid? Maybe return a CreationResult?
				//We don't need to do anything with the returned object.
				GameObject playerGameObject = PlayerFactory.Create(new PlayerEntityCreationContext(dequeuedPlayerSession.Key, dequeuedPlayerSession.Value.SessionContext, new PositionChangeMovementData(0, dequeuedPlayerSession.Value.SpawnPosition, Vector2.zero), EntityPrefab.RemotePlayer, testData));

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Sending player spawn payload Id: {dequeuedPlayerSession.Key.EntityId}");

				//This should always contain it.
				//The reason we check if we're still connected is we want to send the spawn event to the player
				//if they are connected. If the connection has ended, then we want to enqueue an entity deconstruction request.
				if(SessionCollection.Retrieve(dequeuedPlayerSession.Value.SessionContext.ConnectionId).Connection.isConnected)
					//Once added we then need to send to the client a packet indicating its creation
					SpawnPayloadSender.Send(BuildSpawnEventPayload(dequeuedPlayerSession, testData));
				else
					//Warning: This can end up with duplicate enqueued deconstruction requests. But ConnectionId is never shared or reused. So it's ok, it'll just be ingnored in those cases.
					SessionCleanupQueue.Enqueue(new PlayerSessionDeconstructionContext(dequeuedPlayerSession.Value.SessionContext.ConnectionId));

				//TODO: If we want to do anything post-creation with the provide gameobject we could. But we really don't want to at the moment.
			}
		}

		private GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload> BuildSpawnEventPayload(KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext> dequeuedPlayerSession, EntityFieldDataCollection<EntityDataFieldType> testData)
		{
			//TODO: Time stamp
			//TODO: get real movement info
			EntityCreationData data = new EntityCreationData(dequeuedPlayerSession.Key, new PositionChangeMovementData(0, dequeuedPlayerSession.Value.SpawnPosition, Vector2.zero), EntityDataUpdateFactory.Create(new EntityFieldUpdateCreationContext(testData, testData.DataSetIndicationArray)));

			return new GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>(dequeuedPlayerSession.Key, new PlayerSelfSpawnEventPayload(data));
		}
	}
}
