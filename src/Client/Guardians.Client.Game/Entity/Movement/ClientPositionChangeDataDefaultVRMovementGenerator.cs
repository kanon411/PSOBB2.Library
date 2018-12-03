using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class ClientPositionChangeDataDefaultVRMovementGenerator : BaseMovementGenerator<PositionChangeMovementDataDefaultVR>
	{
		/// <inheritdoc />
		public ClientPositionChangeDataDefaultVRMovementGenerator(PositionChangeMovementDataDefaultVR movementData) 
			: base(movementData)
		{
				
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			//We don't need to deal with time when a position change occurs.

			//TODO: This is demo code, we should handle actual movement differently.
			entity.transform.position = MovementData.InitialPosition;

			//We also have the camera look. So we need to set that somehow, we don't have a good way to set the hierarchy of bones/trackers yet
			//TODO: This is hacky, we need a clean efficient way to set this replicated data.
			DemoSettableTrackers trackersComponent = entity.GetComponent<DemoSettableTrackers>();

			trackersComponent.CameraTrackerTransform.localEulerAngles = MovementData.CameraTransform.EulerRotation;
			trackersComponent.CameraTrackerTransform.localPosition = MovementData.CameraTransform.Position;

			trackersComponent.RightHandTrackerTransform.localEulerAngles = MovementData.RightHandTransform.EulerRotation;
			trackersComponent.RightHandTrackerTransform.localPosition = MovementData.RightHandTransform.Position;

			trackersComponent.LeftHandTrackerTransform.localEulerAngles = MovementData.LeftHandTransform.EulerRotation;
			trackersComponent.LeftHandTrackerTransform.localPosition = MovementData.LeftHandTransform.Position;
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{

		}
	}
}
