using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	public sealed class PositionChangeMovementDataDefaultVR : IMovementData
	{
		//TODO: Add support for instant vs lerped

		/// <inheritdoc />
		[ProtoMember(1)]
		public long TimeStamp { get; }

		/// <inheritdoc />
		[ProtoMember(2)]
		public Vector3 InitialPosition { get; }

		[ProtoMember(3)]
		public Vector2 Direction { get; }

		//These objects contain the the transform data related to the camera and the right/left controller
		[ProtoMember(4)]
		public GameObjectEntitySerializableTransform CameraTransform { get; }

		[ProtoMember(5)]
		public GameObjectEntitySerializableTransform LeftHandTransform { get; }

		[ProtoMember(6)]
		public GameObjectEntitySerializableTransform RightHandTransform { get; }

		/// <inheritdoc />
		public PositionChangeMovementDataDefaultVR(long timeStamp, 
			Vector3 initialPosition, 
			Vector2 direction,
			[NotNull] GameObjectEntitySerializableTransform cameraTransform,
			[NotNull] GameObjectEntitySerializableTransform leftHandTransform,
			[NotNull] GameObjectEntitySerializableTransform rightHandTransform)
		{
			TimeStamp = timeStamp;
			InitialPosition = initialPosition;
			Direction = direction;
			CameraTransform = cameraTransform ?? throw new ArgumentNullException(nameof(cameraTransform));
			LeftHandTransform = leftHandTransform ?? throw new ArgumentNullException(nameof(leftHandTransform));
			RightHandTransform = rightHandTransform ?? throw new ArgumentNullException(nameof(rightHandTransform));
		}

		protected PositionChangeMovementDataDefaultVR()
		{

		}
	}
}
