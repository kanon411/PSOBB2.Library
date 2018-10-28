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
		private IReadonlyEntityGuidMappable<GameObject> GameObjectMap { get; }

		private IEntityGuidMappable<MovementInformation> MovementInformationMap { get; }

		/// <inheritdoc />
		public MovementUpdateMessageHandler(ILog logger, IReadonlyEntityGuidMappable<GameObject> gameObjectMap, IEntityGuidMappable<MovementInformation> movementInformationMap) 
			: base(logger)
		{
			GameObjectMap = gameObjectMap;
			MovementInformationMap = movementInformationMap;
		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, MovementDataUpdateEventPayload payload)
		{
			if(!payload.HasMovementData)
				return Task.CompletedTask;

			foreach(var movementUpdate in payload.MovementDatas)
			{
				MovementInformationMap[movementUpdate.EntityGuid] = movementUpdate.InitialMovementData;

				//TODO: This is demo code, we should handle actual movement differently.
				GameObjectMap[movementUpdate.EntityGuid].transform.position = movementUpdate.InitialMovementData.CurrentPosition;

				//This is just a hacky little thing we're using for the demo
				GameObjectMap[movementUpdate.EntityGuid].GetComponent<DemoRemotePlayerInputController>().RecalculateDemoDirection(movementUpdate.InitialMovementData.Direction);
			}

			return Task.CompletedTask;
		}
	}
}
