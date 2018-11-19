using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	//TODO: Refactor, this is just for testing
	public sealed class EntityDataUpdateManager : IGameTickable
	{
		private IPlayerEntityGuidEnumerable PlayerGuids { get; }

		private IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> SessionMappable { get; }

		private IReadonlyEntityGuidMappable<InterestCollection> GuidToInterestCollectionMappable { get; }

		private IFactoryCreatable<FieldValueUpdate, IChangeTrackableEntityDataCollection> UpdateFactory { get; }

		private IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackingCollections { get; }

		/// <inheritdoc />
		public EntityDataUpdateManager(
			[NotNull] IPlayerEntityGuidEnumerable playerGuids, 
			IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> sessionMappable, 
			IReadonlyEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable, 
			IFactoryCreatable<FieldValueUpdate, IChangeTrackableEntityDataCollection> updateFactory, 
			IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackingCollections)
		{
			PlayerGuids = playerGuids ?? throw new ArgumentNullException(nameof(playerGuids));
			SessionMappable = sessionMappable;
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable;
			UpdateFactory = updateFactory;
			ChangeTrackingCollections = changeTrackingCollections;
		}

		/// <inheritdoc />
		public void Tick()
		{
			//For every player we need to do some processing so that we can entity data update values
			foreach(var guid in PlayerGuids)
			{
				InterestCollection interest = GuidToInterestCollectionMappable[guid];

				//Even if we only know ourselves we should do this anyway
				//so that the client can receieve entity data changes about itself

				//TODO: We probably won't send an update about ALL entites, so this is some wasted allocations and time
				List<EntityAssociatedData<FieldValueUpdate>> updates = new List<EntityAssociatedData<FieldValueUpdate>>(interest.ContainedEntities.Count);

				foreach(var interestingEntityGuid in interest.ContainedEntities)
				{
					//Don't build an update for entities that don't have any changes
					if(!ChangeTrackingCollections[interestingEntityGuid].HasPendingChanges)
						continue;

					//TODO: We should cache this update value so we don't need to recompute it for ALL players who are interested
					//This is the update collection for the particular Entity with guid interestingEntityGuid
					FieldValueUpdate update = UpdateFactory.Create(ChangeTrackingCollections[interestingEntityGuid]);

					updates.Add(new EntityAssociatedData<FieldValueUpdate>(interestingEntityGuid, update));
				}

				SessionMappable[guid].SendMessageImmediately(new FieldValueUpdateEvent(updates.ToArray()));
			}

			foreach(var dataEntityCollection in ChangeTrackingCollections.Values)
				dataEntityCollection.ClearTrackedChanges();
		}
	}
}
