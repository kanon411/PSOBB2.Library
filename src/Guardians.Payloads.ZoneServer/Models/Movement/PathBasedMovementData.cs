using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Movement data block used for path based movement.
	/// </summary>
	[ProtoContract]
	public sealed class PathBasedMovementData : IMovementData
	{
		/// <inheritdoc />
		[ProtoMember(1)]
		public long TimeStamp { get; private set; }

		//Ignore this, we use the array (path) to show initial position
		/// <inheritdoc />
		[ProtoIgnore]
		public Vector3 InitialPosition => _MovementPath[0]; //TODO: We should never get an empty path, but we may want to handle this more gracefully

		/// <summary>
		/// The array of points that make up the movement path.
		/// </summary>
		[ProtoMember(2)]
		private Vector3[] _MovementPath;

		/// <summary>
		/// The movement path sent.
		/// </summary>
		[ProtoIgnore]
		public IReadOnlyCollection<Vector3> MovementPath => _MovementPath;

		/// <inheritdoc />
		public PathBasedMovementData([NotNull] Vector3[] movementPath, long timeStamp)
		{
			if(timeStamp <= 0) throw new ArgumentOutOfRangeException(nameof(timeStamp));

			_MovementPath = movementPath ?? throw new ArgumentNullException(nameof(movementPath));
			TimeStamp = timeStamp;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected PathBasedMovementData()
		{
			
		}
	}
}
