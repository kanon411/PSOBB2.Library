using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace PSOBB
{
	//TODO: Rewrite: We need to rewrite this so that this stuff doesn't block networking AND runs IN ORDER on the main thread.
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class FieldValueUpdateEventHandler : BaseZoneClientGameMessageHandler<FieldValueUpdateEvent>
	{
		private IReadonlyEntityGuidMappable<IEntityDataFieldContainer> EntityDataContainerMap { get; }

		/// <inheritdoc />
		public FieldValueUpdateEventHandler(ILog logger,
			IReadonlyEntityGuidMappable<IEntityDataFieldContainer> entityDataContainerMap) 
			: base(logger)
		{
			EntityDataContainerMap = entityDataContainerMap ?? throw new ArgumentNullException(nameof(entityDataContainerMap));
		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, FieldValueUpdateEvent payload)
		{
			//Assume the update fields aren't null and there is at least 1
			foreach(EntityAssociatedData<FieldValueUpdate> update in payload.FieldValueUpdates)
			{
				//TODO: We shouldn't assume we know the entity, but technically we should based on order of server-side events.
				IEntityDataFieldContainer entityDataContainer = EntityDataContainerMap[update.EntityGuid];

				//We have to lock here otherwise we could encounter race conditions with the
				//change tracking system.
				lock(entityDataContainer.SyncObj)
					foreach(var entry in update.Data.FieldValueUpdateMask
						.EnumerateSetBitsByIndex()
						.Zip(update.Data.FieldValueUpdates, (setIndex, value) => new {setIndex, value}))
					{
						entityDataContainer.SetFieldValue(entry.setIndex, entry.value);
					}
			}

			return Task.CompletedTask;
		}
	}
}
