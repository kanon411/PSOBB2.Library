using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class ClientPositionChangeWithLookMovementGenerator : BaseMovementGenerator<PositionChangeMovementDataWithLook>
	{
		/// <inheritdoc />
		public ClientPositionChangeWithLookMovementGenerator(PositionChangeMovementDataWithLook movementData) 
			: base(movementData)
		{
			
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			//We don't need to deal with time when a position change occurs.

			//TODO: This is demo code, we should handle actual movement differently.
			entity.transform.SetPositionAndRotation(MovementData.InitialPosition, Quaternion.Euler(Vector3.up * MovementData.YAxisRotation));

			//We also have the camera look. So we need to set that somehow, we don't have a good way to set the hierarchy of bones/trackers yet
			//TODO: This is hacky, we need a clean efficient way to set this replicated data.
			entity.GetComponent<DemoSettableTrackers>().CameraTrackerTransform.localEulerAngles = MovementData.CameraLookDirection;
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{

		}
	}
}
