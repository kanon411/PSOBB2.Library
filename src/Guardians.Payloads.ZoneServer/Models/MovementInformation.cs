using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	//Actually based on World of Warcraft movement packets
	[ProtoContract]
	public sealed class MovementInformation
	{
		//TODO: Should we send some flags? Maybe 1 bit indicating if this is self?

		//TODO: We may want to make this optional
		/// <summary>
		/// The new position for the mover.
		/// </summary>
		[ProtoMember(1, IsRequired = false)]
		public Vector3 CurrentPosition { get; }

		//TODO: A timestamp would be helpful for prediction and rewinding
		/// <summary>
		/// Rotation around the Y+ (up) axis.
		/// </summary>
		[ProtoMember(2)]
		public float Orientation { get; }

		[ProtoMember(3)]
		public Vector2 Direction { get; }

		/// <inheritdoc />
		public MovementInformation(Vector3 currentPosition, float orientation, Vector2 direction)
		{
			CurrentPosition = currentPosition;
			Orientation = orientation;
			Direction = direction;
		}

		protected MovementInformation()
		{

		}
	}
}
