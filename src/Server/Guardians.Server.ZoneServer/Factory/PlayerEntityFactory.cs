using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	public sealed class PlayerEntityFactory : IFactoryCreatable<GameObject, PlayerEntityCreationContext>
	{
		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> GuidToSessionMappable { get; }

		private IEntityGuidMappable<InterestCollection> GuidToInterestCollectionMappable { get; }

		private IEntityGuidMappable<MovementInformation> GuidToMovementInfoMappable { get; }

		private IGameObjectToEntityMappable GameObjectToEntityMap { get; }

		//Sceneject GameObject factory
		private IGameObjectFactory ObjectFactory { get; }

		private IDictionary<int, NetworkEntityGuid> ConnectionIDToControllingEntityMap { get; }

		/// <inheritdoc />
		public PlayerEntityFactory([NotNull] IEntityGuidMappable<GameObject> guidToGameObjectMappable, 
			[NotNull] IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> guidToSessionMappable, 
			[NotNull] IEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable, 
			[NotNull] IEntityGuidMappable<MovementInformation> guidToMovementInfoMappable,
			[NotNull] IGameObjectFactory objectFactory,
			[NotNull] IGameObjectToEntityMappable gameObjectToEntityMap, 
			[NotNull] IDictionary<int, NetworkEntityGuid> connectionIdToControllingEntityMap)
		{
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToSessionMappable = guidToSessionMappable ?? throw new ArgumentNullException(nameof(guidToSessionMappable));
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable ?? throw new ArgumentNullException(nameof(guidToInterestCollectionMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
			ObjectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			GameObjectToEntityMap = gameObjectToEntityMap ?? throw new ArgumentNullException(nameof(gameObjectToEntityMap));
			ConnectionIDToControllingEntityMap = connectionIdToControllingEntityMap ?? throw new ArgumentNullException(nameof(connectionIdToControllingEntityMap));
		}

		/// <inheritdoc />
		public GameObject Create(PlayerEntityCreationContext context)
		{
			GuidToSessionMappable.Add(context.EntityGuid, context.SessionContext.ZoneSession);
			ConnectionIDToControllingEntityMap.Add(context.SessionContext.ConnectionId, context.EntityGuid);

			//TODO: We should handle prefabs on the server-side better.
			GameObject playerEntityPrefab = Resources.Load<GameObject>("Prefabs/PlayerEntity");
			GameObject playerEntityGameObject = ObjectFactory.Create(playerEntityPrefab);

			GameObjectToEntityMap.ObjectToEntityMap.Add(playerEntityGameObject, context.EntityGuid);

			InterestCollection playerInterestCollection = new InterestCollection();

			//directly add ourselves so we don't become interest in ourselves after spawning
			playerInterestCollection.Add(context.EntityGuid);

			//We just create our own manaul interest collection here.
			GuidToInterestCollectionMappable.Add(context.EntityGuid, playerInterestCollection);

			//TODO: Better handle initial movement/position data
			GuidToMovementInfoMappable.Add(context.EntityGuid, new MovementInformation(Vector3.zero, 0));
			GuidToGameObjectMappable.Add(context.EntityGuid, playerEntityGameObject);

			return playerEntityGameObject;
		}
	}
}
