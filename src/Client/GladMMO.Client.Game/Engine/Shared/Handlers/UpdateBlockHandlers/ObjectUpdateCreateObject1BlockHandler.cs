using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(INetworkEntityVisibleEventSubscribable))]
	public sealed class ObjectUpdateCreateObject1BlockHandler : BaseObjectUpdateBlockHandler<ObjectUpdateCreateObject1Block>, INetworkEntityVisibleEventSubscribable
	{
		/// <inheritdoc />
		public event EventHandler<NetworkEntityNowVisibleEventArgs> OnNetworkEntityNowVisible;

		public IEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackableCollection { get; }

		public IEntityGuidMappable<IEntityDataFieldContainer> DataMappable { get; } 

		/// <inheritdoc />
		public ObjectUpdateCreateObject1BlockHandler(
			ILog logger, 
			IEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackableCollection,
			[NotNull] IEntityGuidMappable<IEntityDataFieldContainer> dataMappable) 
			: base(ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, logger)
		{
			ChangeTrackableCollection = changeTrackableCollection;
			DataMappable = dataMappable ?? throw new ArgumentNullException(nameof(dataMappable));
		}

		public IEntityDataFieldContainer Test(ObjectCreationData creationData)
		{
			//TODO: We could pool this.
			//we actually CAN'T use the field enum length or count. Since TrinityCore may send additional bytes at the end so that
			//it's evently divisible by 32.
			byte[] internalEntityDataBytes = new byte[creationData.ObjectValuesCollection.UpdateMask.Length * sizeof(int)]; 
			IEntityDataFieldContainer t = new EntityFieldDataCollection<EUnitFields>(creationData.ObjectValuesCollection.UpdateMask, internalEntityDataBytes);

			int updateDiffIndex = 0;
			foreach(int setIndex in t.DataSetIndicationArray.EnumerateSetBitsByIndex())
			{
				//The way wow works is these are 4 byte chunks
				for(int i = 0; i < 4; i++)
					internalEntityDataBytes[setIndex * sizeof(int) + i] = creationData.ObjectValuesCollection.UpdateDiffValues[updateDiffIndex * sizeof(int) + i];

				updateDiffIndex++;
			}

			return t;
		}

		/// <inheritdoc />
		public override void HandleUpdateBlock([NotNull] ObjectUpdateCreateObject1Block updateBlock)
		{
			if(updateBlock == null) throw new ArgumentNullException(nameof(updateBlock));

			Logger.Info($"Attempting to Spawn: {updateBlock.CreationData.CreationObjectType}");

			switch(updateBlock.CreationData.CreationObjectType)
			{
				case ObjectType.Object:
					break;
				case ObjectType.Item:
					break;
				case ObjectType.Container:
					break;
				case ObjectType.Unit:
					break;
				case ObjectType.Player:
					//HelloKitty: This is the special case of the local player spawning into the world.
					if(updateBlock.CreationData.MovementData.UpdateFlags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_SELF))
					{
						if(Logger.IsInfoEnabled)
							Logger.Info($"Recieved local player spawn data. Id:{updateBlock.CreationData.CreationGuid.CurrentObjectGuid}");

						DataMappable[new ObjectGuid(updateBlock.CreationData.CreationGuid)] = Test(updateBlock.CreationData);
						//TODO: This is just a test
						ChangeTrackableCollection[new ObjectGuid(updateBlock.CreationData.CreationGuid)] = new ChangeTrackingEntityFieldDataCollectionDecorator(DataMappable[new ObjectGuid(updateBlock.CreationData.CreationGuid)], updateBlock.CreationData.ObjectValuesCollection.UpdateMask);

						//Now we broadcast that an entity is now visible.
						OnNetworkEntityNowVisible?.Invoke(this, new NetworkEntityNowVisibleEventArgs(new ObjectGuid(updateBlock.CreationData.CreationGuid)));
					}
					break;
				case ObjectType.GameObject:
					break;
				case ObjectType.DynamicObject:
					break;
				case ObjectType.Corpse:
					break;
				case ObjectType.AreaTrigger:
					break;
				case ObjectType.SceneObject:
					break;
				case ObjectType.Conversation:
					break;
				case ObjectType.Map:
					break;
				default:
					throw new ArgumentOutOfRangeException($"Unable to handle the creation of ObjectType: {updateBlock.UpdateType}");
			}
		}
	}
}
