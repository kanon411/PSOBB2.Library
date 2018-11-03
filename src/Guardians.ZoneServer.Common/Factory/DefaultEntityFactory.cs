using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;
using JetBrains.Annotations;

namespace Guardians
{
	public class DefaultEntityFactory<TCreationContext> : IFactoryCreatable<GameObject, TCreationContext>
		where TCreationContext : IEntityCreationContext
	{
		private ILog Logger { get; }

		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<IMovementData> GuidToMovementInfoMappable { get; }

		private IGameObjectToEntityMappable GameObjectToEntityMap { get; }

		//Sceneject GameObject factory
		private IGameObjectFactory ObjectFactory { get; }

		private IFactoryCreatable<GameObject, EntityPrefab> PrefabFactory { get; }

		/// <inheritdoc />
		public DefaultEntityFactory(
			ILog logger, 
			IEntityGuidMappable<GameObject> guidToGameObjectMappable, 
			IEntityGuidMappable<IMovementData> guidToMovementInfoMappable, 
			IGameObjectToEntityMappable gameObjectToEntityMap, 
			IGameObjectFactory objectFactory,
			IFactoryCreatable<GameObject, EntityPrefab> prefabFactory)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
			GameObjectToEntityMap = gameObjectToEntityMap ?? throw new ArgumentNullException(nameof(gameObjectToEntityMap));
			ObjectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			PrefabFactory = prefabFactory ?? throw new ArgumentNullException(nameof(prefabFactory));
		}

		/// <inheritdoc />
		public GameObject Create(TCreationContext context)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Creating entity. Type: {context.EntityGuid.EntityType} Id: {context.EntityGuid.EntityId}");

			//load the entity's prefab from the factory
			GameObject prefab = PrefabFactory.Create(context.PrefabType);
			GameObject entityGameObject = ObjectFactory.Create(prefab, context.MovementData.InitialPosition, Quaternion.Euler(0, 0, 0));

			GameObjectToEntityMap.ObjectToEntityMap.Add(entityGameObject, context.EntityGuid);

			//TODO: Better handle initial movement/position data
			GuidToMovementInfoMappable.Add(context.EntityGuid, context.MovementData);

			GuidToGameObjectMappable.Add(context.EntityGuid, entityGameObject);

			return entityGameObject;
		}
	}
}
