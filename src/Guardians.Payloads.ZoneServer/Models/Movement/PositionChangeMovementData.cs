using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Movement data that can be used to denote
	/// just a change in world position.
	/// </summary>
	[ProtoContract]
	public sealed class PositionChangeMovementData : IMovementData
	{
		//TODO: Add support for instant vs lerped

		/// <inheritdoc />
		[ProtoMember(1)]
		public long TimeStamp { get; }

		/// <inheritdoc />
		[ProtoMember(2)]
		public Vector3 InitialPosition { get; }

		/// <inheritdoc />
		public PositionChangeMovementData(long timeStamp, Vector3 initialPosition)
		{
			if(timeStamp <= 0) throw new ArgumentOutOfRangeException(nameof(timeStamp));
			TimeStamp = timeStamp;
			InitialPosition = initialPosition;
		}

		protected PositionChangeMovementData()
		{
			
		}
	}
}
