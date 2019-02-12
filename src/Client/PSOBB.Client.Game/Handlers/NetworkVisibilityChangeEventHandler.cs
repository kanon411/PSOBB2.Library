using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Dissonance;
using GladNet;
using UnityEngine;

namespace PSOBB
{
	public sealed class NetworkVisibilityChangeEventHandler : BaseZoneClientGameMessageHandler<NetworkObjectVisibilityChangeEventPayload>
	{
		private IFactoryCreatable<GameObject, DefaultEntityCreationContext> EntityFactory { get; }

		private IObjectDestructorable<NetworkEntityGuid> EntityDestructor { get; }

		private IReadonlyEntityGuidMappable<GameObject> KnownEntites { get; }

		private IReadonlyNetworkTimeService TimeService { get; }


		private IReadonlyEntityGuidMappable<IEntityDataFieldContainer> EntityDataContainerMap { get; }

		/// <inheritdoc />
		public NetworkVisibilityChangeEventHandler(
			ILog logger,
			IFactoryCreatable<GameObject, DefaultEntityCreationContext> entityFactory,
			IObjectDestructorable<NetworkEntityGuid> entityDestructor,
			IReadonlyEntityGuidMappable<GameObject> knownEntites,
			IReadonlyNetworkTimeService timeService,
			[NotNull] IReadonlyEntityGuidMappable<IEntityDataFieldContainer> entityDataContainerMap)
			: base(logger)
		{
			EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			KnownEntites = knownEntites ?? throw new ArgumentNullException(nameof(knownEntites));
			TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
			EntityDataContainerMap = entityDataContainerMap ?? throw new ArgumentNullException(nameof(entityDataContainerMap));
		}

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, NetworkObjectVisibilityChangeEventPayload payload)
		{
			//TODO: We don't want to have to do this, we should queue this up for handling on the main thread.
			//Must happen from the main thread
			await new UnityYieldAwaitable();

			foreach(var entity in payload.EntitiesToCreate)
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Encountered new entity: {entity.EntityGuid}");
			}

			foreach(var entity in payload.OutOfRangeEntities)
			{
				if(Logger.IsErrorEnabled)
					Logger.Debug($"Leaving entity: {entity}");
			}

			//Assume it's a player for now
			foreach(var creationData in payload.EntitiesToCreate)
			{
				//TODO: Right now we're creating a temporary entity data collection.
				EntityFieldDataCollection<EntityDataFieldType> testData = new EntityFieldDataCollection<EntityDataFieldType>();

				try
				{
					GameObject entityRootObject = EntityFactory.Create(new DefaultEntityCreationContext(creationData.EntityGuid, GetInitialMovementData(creationData), ComputePrefabTypeFromGuid(creationData.EntityGuid), testData));

					SetInitialFieldValues(creationData);
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Create Entity: {creationData.EntityGuid} Exception: {e.Message}\n\nStack: {e.StackTrace}");
					throw;
				}
			}

			foreach(var destroyData in payload.OutOfRangeEntities)
			{
				//If we don't know it, we likely encountered the rare edge case that is the result
				//of some hacks that were added to keep the wheels from falling off.
				//These will eventually be fixed, but for now we should just skip ones we don't know
				//because we can't remove what we don't know.
				if(!KnownEntites.ContainsKey(destroyData))
					continue;

				EntityDestructor.Destroy(destroyData);
			}
		}

		private void SetInitialFieldValues(EntityCreationData creationData)
		{
			//TODO: We need a better way to handle initial field values, this is a disaster.
			IEntityDataFieldContainer entityDataContainer = EntityDataContainerMap[creationData.EntityGuid];

			foreach(var entry in creationData.InitialFieldValues.FieldValueUpdateMask
				.EnumerateSetBitsByIndex()
				.Zip(creationData.InitialFieldValues.FieldValueUpdates, (setIndex, value) => new { setIndex, value }))
			{
				entityDataContainer.SetFieldValue(entry.setIndex, entry.value);
			}
		}

		private IMovementData GetInitialMovementData(EntityCreationData creationData)
		{
			//TODO: Remove path debug testing.
			//If it's not a player, let's return a debug path data.
			/*if(creationData.EntityGuid.EntityType != EntityType.Player)
				return new PathBasedMovementData(new Vector3[]
				{
					new Vector3(2,3,4),
					new Vector3(-5, 1, 5), 
					new Vector3(-5, 1, 17), 
					new Vector3(10, 1, 17), 
					new Vector3(10, 1, 5) 
				}, TimeService.CurrentRemoteTime); //TODO: This only works on local*/

			return creationData.InitialMovementData;
		}

		private EntityPrefab ComputePrefabTypeFromGuid(NetworkEntityGuid creationDataEntityGuid)
		{
			switch(creationDataEntityGuid.EntityType)
			{
				case EntityType.None:
					return EntityPrefab.Unknown;
				case EntityType.Player:
					return EntityPrefab.RemotePlayer;
				case EntityType.GameObject:
					return EntityPrefab.Unknown;
				case EntityType.Npc:
					return EntityPrefab.NetworkNpc;
			}

			return EntityPrefab.Unknown;
		}
	}
}