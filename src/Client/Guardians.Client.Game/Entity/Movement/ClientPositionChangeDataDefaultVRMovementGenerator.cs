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

			trackersComponent.CameraTrackerTransform.eulerAngles = MovementData.CameraTransform.EulerRotation;
			trackersComponent.CameraTrackerTransform.position = MovementData.CameraTransform.Position;

			trackersComponent.RightHandTrackerTransform.eulerAngles = MovementData.RightHandTransform.EulerRotation;
			trackersComponent.RightHandTrackerTransform.position = MovementData.RightHandTransform.Position;

			trackersComponent.LeftHandTrackerTransform.eulerAngles = MovementData.LeftHandTransform.EulerRotation;
			trackersComponent.LeftHandTrackerTransform.position = MovementData.LeftHandTransform.Position;

			//TODO: Remove debugging
			Debug.Log($"Recieved VR Update: {MovementData}");
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{

		}
	}
}
