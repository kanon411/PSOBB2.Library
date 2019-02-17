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
	[AdditionalRegisterationAs(typeof(INetworkEntityVisibilityLostEventSubscribable))]
	[AdditionalRegisterationAs(typeof(INetworkEntityVisibleEventSubscribable))]
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class NetworkVisibilityChangeEventHandler : BaseZoneClientGameMessageHandler<NetworkObjectVisibilityChangeEventPayload>, 
		INetworkEntityVisibleEventSubscribable, 
		INetworkEntityVisibilityLostEventSubscribable
	{
		/// <inheritdoc />
		public event EventHandler<NetworkEntityNowVisibleEventArgs> OnNetworkEntityNowVisible;

		/// <inheritdoc />
		public event EventHandler<NetworkEntityVisibilityLostEventArgs> OnNetworkEntityVisibilityLost;

		/// <inheritdoc />
		public NetworkVisibilityChangeEventHandler(ILog logger)
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, NetworkObjectVisibilityChangeEventPayload payload)
		{
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

				//We set the initial values off the main thread, it is much better that way.
				//However, that means initial values won't dispatch OnChange stuff.
				SetInitialFieldValues(creationData, testData);

				OnNetworkEntityNowVisible?.Invoke(this, new NetworkEntityNowVisibleEventArgs(creationData.EntityGuid, creationData, testData));
			}

			foreach(var destroyData in payload.OutOfRangeEntities)
			{
				OnNetworkEntityVisibilityLost?.Invoke(this, new NetworkEntityVisibilityLostEventArgs(destroyData));
			}

			return Task.CompletedTask;
		}

		private void SetInitialFieldValues([NotNull] EntityCreationData creationData, [NotNull] IEntityDataFieldContainer dataContainer)
		{
			if(creationData == null) throw new ArgumentNullException(nameof(creationData));
			if(dataContainer == null) throw new ArgumentNullException(nameof(dataContainer));

			//TODO: We need a better way to handle initial field values, this is a disaster.
			foreach(var entry in creationData.InitialFieldValues.FieldValueUpdateMask
				.EnumerateSetBitsByIndex()
				.Zip(creationData.InitialFieldValues.FieldValueUpdates, (setIndex, value) => new { setIndex, value }))
			{
				dataContainer.SetFieldValue(entry.setIndex, entry.value);
			}
		}
	}
}