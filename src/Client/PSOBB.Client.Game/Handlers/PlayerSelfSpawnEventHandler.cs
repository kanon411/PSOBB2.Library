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
	/// <summary>
	/// The handler for the player spawn event
	/// the server sends once it enter's the client into the world.
	/// </summary>
	public sealed class PlayerSelfSpawnEventHandler : BaseZoneClientGameMessageHandler<PlayerSelfSpawnEventPayload>
	{
		private IFactoryCreatable<GameObject, DefaultEntityCreationContext> PlayerFactory { get; }

		private IReadOnlyCollection<IGameInitializable> Initializables { get; }

		private ILocalPlayerDetails LocalPlayerDetails { get; }

		/// <inheritdoc />
		public PlayerSelfSpawnEventHandler(
			ILog logger, 
			IFactoryCreatable<GameObject, DefaultEntityCreationContext> playerFactory,
			IReadOnlyCollection<IGameInitializable> initializables,
			ILocalPlayerDetails localPlayerDetails)
			: base(logger)
		{
			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
			Initializables = initializables ?? throw new ArgumentNullException(nameof(initializables));
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
		}

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, PlayerSelfSpawnEventPayload payload)
		{
			//TODO: Actually handle this. Right now it's just demo code, it actually could fail.
			if(Logger.IsInfoEnabled)
				Logger.Info($"Recieved server commanded PlayerSpawn. Player GUID: {payload.CreationData.EntityGuid} Position: {payload.CreationData.InitialMovementData.InitialPosition}");

			EntityFieldDataCollection<EntityDataFieldType> entityData = CreateEntityDataCollectionFromPayload(payload.CreationData.InitialFieldValues);

			LogEntityDataFields(payload);

			await new UnityYieldAwaitable();

			//Don't do any checks for now, we just spawn
			GameObject playerGameObject = PlayerFactory.Create(new DefaultEntityCreationContext(payload.CreationData.EntityGuid, payload.CreationData.InitialMovementData, EntityPrefab.LocalPlayer, entityData));

			//Set local player entity guid, lots of dependencies need this set to work.
			LocalPlayerDetails.LocalPlayerGuid = payload.CreationData.EntityGuid;

			//Call all OnGameInitializables
			foreach(var init in Initializables)
				await init.OnGameInitialized()
					.ConfigureAwait(false);

			//TODO: We need to make this the first packet, or couple of packets. We don't want to do this inbetween potentially slow operatons.
			await context.PayloadSendService.SendMessageImmediately(new ServerTimeSyncronizationRequestPayload(DateTime.UtcNow.Ticks))
				.ConfigureAwait(false);
		}

		private void LogEntityDataFields(PlayerSelfSpawnEventPayload payload)
		{
			StringBuilder entityFieldDataString = new StringBuilder("Entity Field Data:\n");

			foreach(var entry in payload.CreationData.InitialFieldValues.FieldValueUpdateMask
				.EnumerateSetBitsByIndex()
				.Zip(payload.CreationData.InitialFieldValues.FieldValueUpdates, (setIndex, value) => new { setIndex, value }))
			{
				entityFieldDataString.Append($"Index: {(EntityDataFieldType)entry.setIndex}:{entry.setIndex} Value(int): {entry.value}\n");
			}

			if(Logger.IsDebugEnabled)
				Logger.Debug(entityFieldDataString.ToString());
		}

		private static EntityFieldDataCollection<EntityDataFieldType> CreateEntityDataCollectionFromPayload(FieldValueUpdate fieldUpdateValues)
		{
			//TODO: We need better initial handling for entity data
			EntityFieldDataCollection<EntityDataFieldType> entityData = new EntityFieldDataCollection<EntityDataFieldType>();

			int currentSentIndex = 0;
			foreach(var setIndex in fieldUpdateValues.FieldValueUpdateMask.EnumerateSetBitsByIndex())
			{
				entityData.SetFieldValue(setIndex, fieldUpdateValues.FieldValueUpdates.ElementAt(currentSentIndex));
				currentSentIndex++;
			}

			return entityData;
		}
	}
}
