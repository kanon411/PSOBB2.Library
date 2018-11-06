using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class PositionChangeMovementGenerator : BaseMovementGenerator<PositionChangeMovementData>
	{
		/// <inheritdoc />
		public PositionChangeMovementGenerator(PositionChangeMovementData movementData) 
			: base(movementData)
		{
			
		}

		/// <inheritdoc />
		protected override void InternalStart(GameObject entity, long currentTime)
		{
			//We don't need to deal with time when a position change occurs.

			//TODO: This is demo code, we should handle actual movement differently.
			entity.transform.position = MovementData.InitialPosition;

			//TODO: We need to handle multiple movement types
			//This is just a hacky little thing we're using for the demo
			entity.GetComponent<DemoRemotePlayerInputController>().RecalculateDemoDirection(MovementData.Direction);
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, float deltaTime)
		{
			//We don't need to do anything in update, position is already changed in start.
		}
	}
}
