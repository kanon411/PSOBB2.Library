﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace PSOBB
{
	[ProtoContract]
	[ProtoInclude(1, typeof(PositionChangeMovementData))]
	[ProtoInclude(2, typeof(PathBasedMovementData))]
	public interface IMovementData
	{
		/// <summary>
		/// The timestamp that a particular movement data
		/// was created at on the server.
		/// </summary>
		[ProtoIgnore]
		long TimeStamp { get; }

		//TODO: Doc better
		/// <summary>
		/// The initial position
		/// that the moving entity started with
		/// before this movement data was created.
		/// </summary>
		[ProtoIgnore]
		Vector3 InitialPosition { get; }
	}
}
