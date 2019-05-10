using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ISelfPlayerSpawnEventSubscribable))]
	[SceneTypeCreate(GameSceneType.DefaultLobby)] //we spawn in lobbies
	public sealed class PlayerSelfSpawnEventPayloadHandler : BaseZoneClientGameMessageHandler<PlayerSelfSpawnEventPayload>, ISelfPlayerSpawnEventSubscribable
	{
		private ILocalPlayerDetails LocalPlayerDetails { get; }

		/// <inheritdoc />
		public event EventHandler<SelfPlayerSpawnEventArgs> OnSelfPlayerSpawnEvent;

		/// <inheritdoc />
		public PlayerSelfSpawnEventPayloadHandler(ILog logger, [NotNull] ILocalPlayerDetails localPlayerDetails) 
			: base(logger)
		{
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
		}

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, PlayerSelfSpawnEventPayload payload)
		{
			//This is kinda a hack, to make this handler continue on the main thread so that dispatching the event
			//will be on the main thread too.
			//Otherwise, we'd need a system for queueing such an event on the main thread and NOT continuting the networking until
			//all events complete.
			//The reason being as SOON as this happens, something COULD happen the player entity to trigger a value change update.
			await new UnityYieldAwaitable();

			//We should ONLY allow this to be the case for THIS HANDLER ONLY. Doing this in other handlers is NOT VIABLE.

			//TODO: Actually handle this. Right now it's just demo code, it actually could fail.
			if(Logger.IsInfoEnabled)
				Logger.Info($"Recieved server commanded PlayerSpawn. Player GUID: {payload.CreationData.EntityGuid} Position: {payload.CreationData.InitialMovementData.InitialPosition}");

			LogEntityDataFields(payload);

			LocalPlayerDetails.LocalPlayerGuid = payload.CreationData.EntityGuid;

			//We should broadcast to interested parties that spawn event has occured.
			OnSelfPlayerSpawnEvent?.Invoke(this, new SelfPlayerSpawnEventArgs(payload.CreationData));

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
	}
}
