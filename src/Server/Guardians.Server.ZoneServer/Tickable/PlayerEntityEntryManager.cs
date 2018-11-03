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
	public sealed class PlayerEntityEntryManager : IGameTickable
	{
		private IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>> PlayerEntitySessionDequeable { get; }

		private IFactoryCreatable<GameObject, PlayerEntityCreationContext> PlayerFactory { get; }

		private INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> SpawnPayloadSender { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public PlayerEntityEntryManager(
			[NotNull] IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>> playerEntitySessionDequeable, 
			[NotNull] IFactoryCreatable<GameObject, PlayerEntityCreationContext> playerFactory,
			[NotNull] INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> spawnPayloadSender,
			[NotNull] ILog logger)
		{
			PlayerEntitySessionDequeable = playerEntitySessionDequeable ?? throw new ArgumentNullException(nameof(playerEntitySessionDequeable));
			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
			SpawnPayloadSender = spawnPayloadSender ?? throw new ArgumentNullException(nameof(spawnPayloadSender));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

				//TODO: Time stamp
				//TODO: We should check if the result is valid? Maybe return a CreationResult?
				//We don't need to do anything with the returned object.
				GameObject playerGameObject = PlayerFactory.Create(new PlayerEntityCreationContext(dequeuedPlayerSession.Key, dequeuedPlayerSession.Value.SessionContext, new PositionChangeMovementData(0, dequeuedPlayerSession.Value.SpawnPosition, Vector2.zero), EntityPrefab.RemotePlayer));

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Sending player spawn payload Id: {dequeuedPlayerSession.Key.EntityId}");

				//Once added we then need to send to the client a packet indicating its creation
				SpawnPayloadSender.Send(BuildSpawnEventPayload(dequeuedPlayerSession));

				//TODO: If we want to do anything post-creation with the provide gameobject we could. But we really don't want to at the moment.
			}
		}

		private static GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload> BuildSpawnEventPayload(KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext> dequeuedPlayerSession)
		{
			//TODO: Time stamp
			//TODO: get real movement info
			EntityCreationData data = new EntityCreationData(dequeuedPlayerSession.Key, new PositionChangeMovementData(0, dequeuedPlayerSession.Value.SpawnPosition, Vector2.zero));

			return new GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>(dequeuedPlayerSession.Key, new PlayerSelfSpawnEventPayload(data));
		}
	}
}
