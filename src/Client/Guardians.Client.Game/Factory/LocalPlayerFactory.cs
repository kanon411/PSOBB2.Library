using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class LocalPlayerFactory : IFactoryCreatable<GameObject, LocalPlayerCreationContext>
	{
		private ILog Logger { get; }

		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<MovementInformation> GuidToMovementInfoMappable { get; }

		private IGameObjectToEntityMappable GameObjectToEntityMap { get; }

		//Sceneject GameObject factory
		private IGameObjectFactory ObjectFactory { get; }

		/// <inheritdoc />
		public LocalPlayerFactory(
			ILog logger, 
			IEntityGuidMappable<GameObject> guidToGameObjectMappable, 
			IEntityGuidMappable<MovementInformation> guidToMovementInfoMappable, 
			IGameObjectToEntityMappable gameObjectToEntityMap, 
			IGameObjectFactory objectFactory)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
			GameObjectToEntityMap = gameObjectToEntityMap ?? throw new ArgumentNullException(nameof(gameObjectToEntityMap));
			ObjectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
		}

		/// <inheritdoc />
		public GameObject Create(LocalPlayerCreationContext context)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Creating local player. GUID: {context.EntityGuid}");

			GameObject playerEntityPrefab = LoadLocalPlayerPrefab();
			GameObject playerEntityGameObject = ObjectFactory.Create(playerEntityPrefab, context.MovementData.CurrentPosition, Quaternion.Euler(0, context.MovementData.Orientation, 0));

			GameObjectToEntityMap.ObjectToEntityMap.Add(playerEntityGameObject, context.EntityGuid);

			//TODO: Better handle initial movement/position data
			GuidToMovementInfoMappable.Add(context.EntityGuid, context.MovementData);

			GuidToGameObjectMappable.Add(context.EntityGuid, playerEntityGameObject);

			return playerEntityGameObject;
		}

		private static GameObject LoadLocalPlayerPrefab()
		{
			string prefabString = "Prefabs/LocalPlayerAvatar";

			//TODO: We should handle prefabs better
			GameObject prefab = Resources.Load<GameObject>(prefabString);

			if(prefab == null)
				throw new InvalidOperationException($"Failed to load Local Player Prefab at Path: {prefabString}");

			return prefab;
		}
	}
}
