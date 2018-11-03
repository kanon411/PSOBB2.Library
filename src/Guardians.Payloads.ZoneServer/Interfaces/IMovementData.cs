using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	[ProtoContract]
	[ProtoInclude(1, typeof(PositionChangeMovementData))]
	public interface IMovementData
	{
		/// <summary>
		/// The timestamp that a particular movement data
		/// was created at on the server.
		/// </summary>
		long TimeStamp { get; }

		//TODO: Doc better
		/// <summary>
		/// The initial position
		/// that the moving entity started with
		/// before this movement data was created.
		/// </summary>
		Vector3 InitialPosition { get; }
	}
}
