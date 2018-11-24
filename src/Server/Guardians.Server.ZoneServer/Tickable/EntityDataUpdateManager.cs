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

		private IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> UpdateFactory { get; }

		private IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackingCollections { get; }

		/// <inheritdoc />
		public EntityDataUpdateManager(
			[NotNull] IPlayerEntityGuidEnumerable playerGuids, 
			IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> sessionMappable, 
			IReadonlyEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable, 
			IFactoryCreatable<FieldValueUpdate, EntityFieldUpdateCreationContext> updateFactory, 
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
					//We want to use the CHANGE TRACKING bitarray for updates. If this was initial discovery we'd use the SIT bitarray to send all set values.
					FieldValueUpdate update = UpdateFactory.Create(new EntityFieldUpdateCreationContext(ChangeTrackingCollections[interestingEntityGuid], ChangeTrackingCollections[interestingEntityGuid].ChangeTrackingArray));

					updates.Add(new EntityAssociatedData<FieldValueUpdate>(interestingEntityGuid, update));
				}

				//It's possible no entity had updates, so we should not send a packet update
				if(updates.Count != 0)
					SessionMappable[guid].SendMessageImmediately(new FieldValueUpdateEvent(updates.ToArray()));
			}

			foreach(var dataEntityCollection in ChangeTrackingCollections.Values)
				dataEntityCollection.ClearTrackedChanges();
		}
	}
}
