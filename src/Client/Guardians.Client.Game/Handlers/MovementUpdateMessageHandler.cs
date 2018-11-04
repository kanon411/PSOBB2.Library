using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace Guardians
{
	public sealed class MovementUpdateMessageHandler : BaseZoneClientGameMessageHandler<MovementDataUpdateEventPayload>
	{
		private IMovementDataHandlerService MovementHandlerService { get; }

		/// <inheritdoc />
		public MovementUpdateMessageHandler(
			ILog logger, 
			IMovementDataHandlerService movementHandlerService) 
			: base(logger)
		{
			MovementHandlerService = movementHandlerService ?? throw new ArgumentNullException(nameof(movementHandlerService));
		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, MovementDataUpdateEventPayload payload)
		{
			if(!payload.HasMovementData)
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Empty movement update packet recieved.");

				return Task.CompletedTask;
			}

			foreach(var movementUpdate in payload.MovementDatas)
			{
				bool result = MovementHandlerService.TryHandleMovement(movementUpdate.EntityGuid, movementUpdate.InitialMovementData);

				if(!result)
				{
					string error = $"Failed to handle Movement Data for Entity: {movementUpdate.EntityGuid} Type: {movementUpdate.InitialMovementData.GetType().Name}";

					if(Logger.IsErrorEnabled)
						Logger.Error(error);

					throw new InvalidOperationException(error);
				}
			}

			return Task.CompletedTask;

			/*if(!payload.HasMovementData)
				return Task.CompletedTask;

			foreach(var movementUpdate in payload.MovementDatas)
			{
				MovementDataMap[movementUpdate.EntityGuid] = movementUpdate.InitialMovementData;

				//TODO: This is demo code, we should handle actual movement differently.
				GameObjectMap[movementUpdate.EntityGuid].transform.position = movementUpdate.InitialMovementData.InitialPosition;

				//TODO: We need to handle multiple movement types
				//This is just a hacky little thing we're using for the demo
				GameObjectMap[movementUpdate.EntityGuid].GetComponent<DemoRemotePlayerInputController>().RecalculateDemoDirection((movementUpdate.InitialMovementData as PositionChangeMovementData).Direction);
			}

			return Task.CompletedTask;*/
			}
	}
}
