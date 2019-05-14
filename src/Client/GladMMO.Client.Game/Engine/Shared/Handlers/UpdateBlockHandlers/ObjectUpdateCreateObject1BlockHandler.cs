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

		/// <inheritdoc />
		public ObjectUpdateCreateObject1BlockHandler(ILog logger, IEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackableCollection) 
			: base(ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, logger)
		{
			ChangeTrackableCollection = changeTrackableCollection;
		}

		public IEntityDataFieldContainer Test(ObjectCreationData creationData)
		{
			/*IEntityDataFieldContainer t = new EntityFieldDataCollection<EUnitFields>(creationData.ObjectValuesCollection.UpdateMask, );

			foreach(var entry in t.DataSetIndicationArray.FieldValueUpdateMask
				.EnumerateSetBitsByIndex()
				.Zip(creationData.InitialFieldValues.FieldValueUpdates, (setIndex, value) => new { setIndex, value }))
			{
				entityDataContainer.SetFieldValue(entry.setIndex, entry.value);
			}*/
			return null;
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
					}

					//TODO: This is just a test
					//ChangeTrackableCollection[new ObjectGuid(updateBlock.CreationData.CreationGuid)] = new ChangeTrackingEntityFieldDataCollectionDecorator(Test());

					//Now we broadcast that an entity is now visible.
					OnNetworkEntityNowVisible?.Invoke(this, new NetworkEntityNowVisibleEventArgs(new ObjectGuid(updateBlock.CreationData.CreationGuid)));
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
