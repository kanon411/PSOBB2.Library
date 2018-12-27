﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class ClientPositionChangeWithLookMovementGenerator : BaseMovementGenerator<PositionChangeMovementDataWithLook>
	{
		private Vector3 ComputedNormalizedMovementDirection;

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

			ComputedNormalizedMovementDirection = (entity.transform.right * MovementData.Direction.x + entity.transform.forward * MovementData.Direction.y);

			//Remove any potential Y value for safety
			ComputedNormalizedMovementDirection = new Vector3(ComputedNormalizedMovementDirection.x, 0, ComputedNormalizedMovementDirection.z).normalized;
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//TODO: We are harcoding speed here, we shouldn't do that.
			ProjectVersionStage.AssertAlpha();
			//TODO: The time syncronization is not working, it's off by like 0.15 seconds for some reason.
			//entity.transform.position = MovementData.InitialPosition + ComputedNormalizedMovementDirection * ComputeTimestampDiffSeconds(currentTime) * 3.0f;
			entity.transform.position += ComputedNormalizedMovementDirection * Time.deltaTime * 3.0f;
		}

		private float ComputeTimestampDiffSeconds(long currentTime)
		{
			long diff = currentTime - MovementData.TimeStamp;

			if(diff < 0)
				Debug.LogError($"Diff Less Than Zero: {diff}");
			else
				Debug.Log($"Diff: {(float)diff / (float)TimeSpan.TicksPerSecond}");

			//Must use ticks per second as time.deltaTime is in seconds
			return (float)(diff < 0 ? 0 : diff) / (float)TimeSpan.TicksPerSecond;
		}
	}
}
