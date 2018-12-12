using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Movement data that can be used to a change in world position with
	/// a change in the looking direction of an entity. (for Head IK)
	/// </summary>
	[ProtoContract]
	public sealed class PositionChangeMovementDataWithLook : IMovementData
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

		/// <summary>
		/// This is the direction that the camera is looking in.
		/// It can be used to control the head anchor for IK so that the avatar will
		/// look in the direction that the camera is looking.
		/// </summary>
		[ProtoMember(4)]
		public Vector3 CameraLookDirection { get; }

		/// <summary>
		/// The rotation around the Y axis.
		/// </summary>
		[ProtoMember(5)]
		public float YAxisRotation { get; }

		/// <inheritdoc />
		public PositionChangeMovementDataWithLook(
			long timeStamp, 
			Vector3 initialPosition, 
			Vector2 direction, 
			Vector3 cameraLookDirection, 
			float yAxisRotation)
		{
			TimeStamp = timeStamp;
			InitialPosition = initialPosition;
			Direction = direction;
			CameraLookDirection = cameraLookDirection;
			YAxisRotation = yAxisRotation;
		}

		protected PositionChangeMovementDataWithLook()
		{

		}
	}
}
