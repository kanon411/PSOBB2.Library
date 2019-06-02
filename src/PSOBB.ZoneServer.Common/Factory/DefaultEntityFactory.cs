﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;
using JetBrains.Annotations;
using Nito.AsyncEx;

namespace PSOBB
{
	//TODO: Server need to be rewritten to no longer require sceneject injection on components.
	//TODO: Refactor this, it's becoming quie bloated and depends on nearly everything.
	public class DefaultEntityFactory<TCreationContext> : IFactoryCreatable<GameObject, TCreationContext>
		where TCreationContext : IEntityCreationContext
	{
		private ILog Logger { get; }

		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<IMovementData> GuidToMovementInfoMappable { get; }

		private IGameObjectToEntityMappable GameObjectToEntityMap { get; }

		private IFactoryCreatable<GameObject, EntityPrefab> PrefabFactory { get; }

		private IMovementDataHandlerService MovementHandlerService { get; }

		private IEntityGuidMappable<IEntityDataFieldContainer> FieldDataContainers { get; }

		private IEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackableEntityDataFieldContainers { get; }

		private IEntityGuidMappable<AsyncReaderWriterLock> EntityAsyncLockMap { get; }

		private IEntityGuidMappable<CharacterController> CharacterControllerMappable { get; }

		/// <inheritdoc />
		public DefaultEntityFactory(
			ILog logger, 
			IEntityGuidMappable<GameObject> guidToGameObjectMappable, 
			IEntityGuidMappable<IMovementData> guidToMovementInfoMappable, 
			IGameObjectToEntityMappable gameObjectToEntityMap, 
			IFactoryCreatable<GameObject, EntityPrefab> prefabFactory,
			IMovementDataHandlerService movementHandlerService, 
			IEntityGuidMappable<IEntityDataFieldContainer> fieldDataContainers, 
			IEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackableEntityDataFieldContainers,
			[NotNull] IEntityGuidMappable<AsyncReaderWriterLock> entityAsyncLockMap,
			[NotNull] IEntityGuidMappable<CharacterController> characterControllerMappable)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
			GameObjectToEntityMap = gameObjectToEntityMap ?? throw new ArgumentNullException(nameof(gameObjectToEntityMap));
			PrefabFactory = prefabFactory ?? throw new ArgumentNullException(nameof(prefabFactory));
			MovementHandlerService = movementHandlerService ?? throw new ArgumentNullException(nameof(movementHandlerService));
			FieldDataContainers = fieldDataContainers;
			ChangeTrackableEntityDataFieldContainers = changeTrackableEntityDataFieldContainers;
			EntityAsyncLockMap = entityAsyncLockMap ?? throw new ArgumentNullException(nameof(entityAsyncLockMap));
			CharacterControllerMappable = characterControllerMappable ?? throw new ArgumentNullException(nameof(characterControllerMappable));
		}

		/// <inheritdoc />
		public GameObject Create(TCreationContext context)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Creating entity. Type: {context.EntityGuid.EntityType} Id: {context.EntityGuid.EntityId}");

			//load the entity's prefab from the factory
			GameObject prefab = PrefabFactory.Create(context.PrefabType);

			GameObject entityGameObject = GameObject.Instantiate(prefab, context.MovementData.InitialPosition, Quaternion.Euler(0, 0, 0));

			if(context.EntityGuid.EntityType == EntityType.Player)
				CharacterControllerMappable[context.EntityGuid] = entityGameObject.GetComponent<CharacterController>();

			GameObjectToEntityMap.ObjectToEntityMap.Add(entityGameObject, context.EntityGuid);

			//TODO: Better handle initial movement/position data
			GuidToMovementInfoMappable.Add(context.EntityGuid, context.MovementData);

			GuidToGameObjectMappable.Add(context.EntityGuid, entityGameObject);

			//TODO: Is it best to do this here?
			if(!MovementHandlerService.TryHandleMovement(context.EntityGuid, context.MovementData))
				throw new InvalidOperationException($"Cannot handle MovementType: {context.MovementData.GetType().Name} for Entity: {context.EntityGuid}");

			//TODO: We need a better way to handle the entity data collection, we're casting and downcasting in afew spots
			//Entity data needs to be change trackable
			var changeTrackableEntityDataCollection = new ChangeTrackingEntityFieldDataCollectionDecorator<EntityDataFieldType>((IEntityDataFieldContainer<EntityDataFieldType>)context.EntityData);

			//Now we should add the entity data to the mappable collection
			//This lets it be looked up in both ways
			FieldDataContainers.Add(context.EntityGuid, changeTrackableEntityDataCollection);
			ChangeTrackableEntityDataFieldContainers.Add(context.EntityGuid, changeTrackableEntityDataCollection);

			EntityAsyncLockMap.Add(new KeyValuePair<NetworkEntityGuid, AsyncReaderWriterLock>(context.EntityGuid, new AsyncReaderWriterLock()));

			return entityGameObject;
		}
	}
}
