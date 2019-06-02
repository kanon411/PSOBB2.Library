﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class EntityDataChangeTrackerTickable : IGameTickable
	{
		private IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackableMap { get; }

		private IEntityDataChangeCallbackService EntityDataCallbackDispatcher { get; }

		/// <inheritdoc />
		public EntityDataChangeTrackerTickable(IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackableMap, IEntityDataChangeCallbackService entityDataCallbackDispatcher)
		{
			ChangeTrackableMap = changeTrackableMap ?? throw new ArgumentNullException(nameof(changeTrackableMap));
			EntityDataCallbackDispatcher = entityDataCallbackDispatcher ?? throw new ArgumentNullException(nameof(entityDataCallbackDispatcher));
		}

		/// <inheritdoc />
		public void Tick()
		{
			//TODO: We can make this faster if we skip entities that have no registered callbacks.
			//The idea is here that we check the changed values, while on the MAIN THREAD
			//and dispatch the changes via a callback registeration service where UI or gameplay systems
			//may register their interest
			foreach(var changeTrackerPair in ChangeTrackableMap)
			{
				if(changeTrackerPair.Value.HasPendingChanges)
				{
					//We have to lock here otherwise we could encounter race conditions with the
					//change tracking system.
					lock(changeTrackerPair.Value.SyncObj)
					{
						//We need to try to dispatch events for each changed value.
						foreach(int changedIndex in changeTrackerPair.Value.ChangeTrackingArray.EnumerateSetBitsByIndex())
						{
							//TODO: We don't REALLY want to lock on the dispatching. This could be a REAL bottleneck in the future. We need to redesign this abit
							//TODO: Might be a better way to handle this API, and provide the value instead of the collection.
							EntityDataCallbackDispatcher.InvokeChangeEvents(changeTrackerPair.Key, (EntityDataFieldType)changedIndex, changeTrackerPair.Value.GetFieldValue<int>((int)changedIndex));
						}

						//After we're done servicing the changes, we should clear the changes.
						changeTrackerPair.Value.ClearTrackedChanges();
					}
				}
			}
		}
	}
}
