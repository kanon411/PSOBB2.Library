using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;
using Glader.Essentials;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(IFactoryCreatable<NetworkEntityNowVisibleEventArgs, ObjectUpdateCreateObject1Block>))]
	[SceneTypeCreateGladMMO(GameSceneType.DefaultLobby)]
	public sealed class NetworkVisibilityCreationBlockToVisibilityEventFactory : IFactoryCreatable<NetworkEntityNowVisibleEventArgs, ObjectUpdateCreateObject1Block>
	{
		public IEntityGuidMappable<IChangeTrackableEntityDataCollection> ChangeTrackableCollection { get; }

		public IEntityGuidMappable<IEntityDataFieldContainer> DataMappable { get; }

		/// <inheritdoc />
		public NetworkVisibilityCreationBlockToVisibilityEventFactory([NotNull] IEntityGuidMappable<IChangeTrackableEntityDataCollection> changeTrackableCollection,
			[NotNull] IEntityGuidMappable<IEntityDataFieldContainer> dataMappable)
		{
			ChangeTrackableCollection = changeTrackableCollection ?? throw new ArgumentNullException(nameof(changeTrackableCollection));
			DataMappable = dataMappable ?? throw new ArgumentNullException(nameof(dataMappable));
		}

		/// <inheritdoc />
		public NetworkEntityNowVisibleEventArgs Create(ObjectUpdateCreateObject1Block context)
		{
			ObjectGuid guid = new ObjectGuid(context.CreationData.CreationGuid);

			var initialContainer = DataMappable[guid] = CreateInitialEntityFieldContainer(context.CreationData);

			//TODO: This is just a test
			ChangeTrackableCollection[guid] = new ChangeTrackingEntityFieldDataCollectionDecorator(initialContainer, context.CreationData.ObjectValuesCollection.UpdateMask);

			return new NetworkEntityNowVisibleEventArgs(guid);
		}

		public IEntityDataFieldContainer CreateInitialEntityFieldContainer(ObjectCreationData creationData)
		{
			//TODO: We could pool this.
			//we actually CAN'T use the field enum length or count. Since TrinityCore may send additional bytes at the end so that
			//it's evently divisible by 32.
			byte[] internalEntityDataBytes = new byte[creationData.ObjectValuesCollection.UpdateMask.Length * sizeof(int)];
			IEntityDataFieldContainer t = new EntityFieldDataCollection<EUnitFields>(creationData.ObjectValuesCollection.UpdateMask, internalEntityDataBytes);

			int updateDiffIndex = 0;
			foreach(int setIndex in t.DataSetIndicationArray.EnumerateSetBitsByIndex())
			{
				//TODO: Would it be faster to buffer copy?
				//The way wow works is these are 4 byte chunks
				for(int i = 0; i < 4; i++)
					internalEntityDataBytes[setIndex * sizeof(int) + i] = creationData.ObjectValuesCollection.UpdateDiffValues[updateDiffIndex * sizeof(int) + i];

				updateDiffIndex++;
			}

			return t;
		}
	}
}
