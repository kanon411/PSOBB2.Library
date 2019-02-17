using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace PSOBB
{
	[AdditionalRegisterationAs(typeof(IPlayerWorldSessionCreatedEventSubscribable))]
	[GameInitializableOrdering(2)]
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class PlayerEntityEntryManager : EventQueueBasedTickable<IPlayerSessionClaimedEventSubscribable, PlayerSessionClaimedEventArgs>, IPlayerWorldSessionCreatedEventSubscribable
	{
		private IFactoryCreatable<GameObject, PlayerEntityCreationContext> PlayerFactory { get; }

		private INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> SpawnPayloadSender { get; }

		private IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> EntityDataUpdateFactory { get; }

		/// <inheritdoc />
		public event EventHandler<PlayerWorldSessionCreationEventArgs> OnPlayerWorldSessionCreated;

		/// <summary>
		/// The collections locking policy.
		/// </summary>
		private GlobalEntityCollectionsLockingPolicy LockingPolicy { get; }

		/// <inheritdoc />
		public PlayerEntityEntryManager(
			IPlayerSessionClaimedEventSubscribable subscriptionService, 
			IFactoryCreatable<GameObject, PlayerEntityCreationContext> playerFactory, 
			INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> spawnPayloadSender, 
			IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> entityDataUpdateFactory,
			ILog logger,
			GlobalEntityCollectionsLockingPolicy lockingPolicy) 
			: base(subscriptionService, false, logger)
		{
			PlayerFactory = playerFactory;
			SpawnPayloadSender = spawnPayloadSender;
			EntityDataUpdateFactory = entityDataUpdateFactory;
			LockingPolicy = lockingPolicy;
		}

		/// <inheritdoc />
		protected override void HandleEvent(PlayerSessionClaimedEventArgs args)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Dequeueing entity creation request for: {args.EntityGuid.EntityType}:{args.EntityGuid.EntityId}");

			//TODO: This is test data
			EntityFieldDataCollection<EntityDataFieldType> testData = new EntityFieldDataCollection<EntityDataFieldType>();

			//TODO: Test values set
			testData.SetFieldValue(EntityDataFieldType.EntityCurrentHealth, 100);
			testData.SetFieldValue(EntityDataFieldType.EntityMaxHealth, 120);

			using(LockingPolicy.WriterLock(null, CancellationToken.None))
			{
				//TODO: Time stamp
				//TODO: We should check if the result is valid? Maybe return a CreationResult?
				//We don't need to do anything with the returned object.
				GameObject playerGameObject = PlayerFactory.Create(new PlayerEntityCreationContext(args.EntityGuid, args.SessionContext, new PositionChangeMovementData(0, args.SpawnPosition, Vector2.zero), EntityPrefab.RemotePlayer, testData));
			}

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Sending player spawn payload Id: {args.EntityGuid.EntityId}");

			OnPlayerWorldSessionCreated?.Invoke(this, new PlayerWorldSessionCreationEventArgs(args.EntityGuid));

			//TODO: If we want to do anything post-creation with the provide gameobject we could. But we really don't want to at the moment.

		}
	}
}
