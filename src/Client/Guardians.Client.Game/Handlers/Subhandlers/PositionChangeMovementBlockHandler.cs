using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Guardians
{
	public sealed class PositionChangeMovementBlockHandler : SpecificMovementTypeHandler<PositionChangeMovementData>
	{
		private IReadonlyEntityGuidMappable<GameObject> GameObjectMap { get; }

		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		/// <inheritdoc />
		public PositionChangeMovementBlockHandler(
			IReadonlyEntityGuidMappable<GameObject> gameObjectMap, 
			IEntityGuidMappable<IMovementData> movementDataMap)
		{
			GameObjectMap = gameObjectMap ?? throw new ArgumentNullException(nameof(gameObjectMap));
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
		}

		/// <inheritdoc />
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PositionChangeMovementData data)
		{
			MovementDataMap[entityGuid] = data;

			//TODO: This is demo code, we should handle actual movement differently.
			GameObjectMap[entityGuid].transform.position = data.InitialPosition;

			//TODO: We need to handle multiple movement types
			//This is just a hacky little thing we're using for the demo
			GameObjectMap[entityGuid].GetComponent<DemoRemotePlayerInputController>().RecalculateDemoDirection(data.Direction);
		}
	}
}
