using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class PathMovementGenerator : BaseMovementGenerator<PathBasedMovementData>
	{
		private sealed class PathState
		{
			/// <summary>
			/// The index of the path the movement generation is on.
			/// </summary>
			public int PathIndex { get; }

			/// <summary>
			/// The initial position to start pathing from to the point
			/// at index <see cref="PathIndex"/>
			/// </summary>
			public long IndexTimeOffset { get; }

			/// <inheritdoc />
			public PathState(int pathIndex, long indexTimeOffset)
			{
				if(pathIndex <= 0) throw new ArgumentOutOfRangeException(nameof(pathIndex));
				if(indexTimeOffset <= 0) throw new ArgumentOutOfRangeException(nameof(indexTimeOffset));

				PathIndex = pathIndex;
				IndexTimeOffset = indexTimeOffset;
			}
		}

		private PathState State { get; set; }

		/// <inheritdoc />
		public PathMovementGenerator(PathBasedMovementData movementData) 
			: base(movementData)
		{

		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			//We don't need to deal with time when a position change occurs.
			//TODO: This is demo code, we should handle actual movement differently.

			//This sets the initial position for the path, offset by the timestamp in the movement data.
			//entity.transform.position;
			State = ComputeInitialPathState(currentTime);

			//The initial point is the movement speed * the initial tick offset from the current point.
			entity.transform.position = MovementData.MovementPath[State.PathIndex] + 
				MovementData.MovementPath[State.PathIndex].normalized * (State.IndexTimeOffset / TimeSpan.TicksPerMillisecond) * (1.0f / 1000f);
		}

		private PathState ComputeInitialPathState(long currentTime)
		{
			//So when a path starts, it may not be on the "initial position" because it could have
			//started pathing long before this movement data was created on the server
			//Therefore we must step forward out simulation to the point where it should be.
			long startTimeDiff = currentTime - this.MovementData.TimeStamp;
			float movementSpeedPerSecond = 1.0f;

			//TODO: Handle case where the start time is actually the timestamp (unrealistic real world)

			//TODO: How should we deal with a negative time diff??
			if(startTimeDiff < 0)
				throw new InvalidOperationException($"Encountered Negative Time Diff. CurrentTime: {currentTime} MovementStamp: {MovementData.TimeStamp}");

			for(int i = 0; i < this.MovementData.MovementPath.Count - 1; i++)
			{
				Vector3 distanceVectorOffset = this.MovementData.MovementPath[i + 1] - this.MovementData.MovementPath[i];

				//Compute how long it would take to travel this distance at the current speed
				//TODO: Handle different speeds
				float distance = distanceVectorOffset.magnitude;

				//This is the time it would have traveled since the timestamp
				float distancedTraveledSince = (startTimeDiff / TimeSpan.TicksPerMillisecond) * (movementSpeedPerSecond / 1000f);

				if(distancedTraveledSince > distance)
				{
					//this means that the time elasped would put us past the initial path point
					//so we must remove the time taken to travel the the initial ray and continue down the path
					startTimeDiff -= (long)((distance) / (1.0f)) * TimeSpan.TicksPerMillisecond; // (x / speed) = time
				}
				else
				{
					//TODO: Case where we're somewhere between the points.
					//Distance Travel / Total Distance = Ratio of how far to move from pointX to pointX+1.
					//Ratio * normalized distanceOffset is the direction from pointX to pointX+1 so we can multiply that times the direction
					//to get the new offset we should move from pointX.
					//Then we have to add the pointX.
					//Vector3 initialPosition = ((distancedTraveledSince / distance) * distanceVectorOffset.normalized) + this.MovementData.MovementPath[i];

					//The current time minus the time taken to reach the current distance traveled from the last point is the offset of how far
					//we are into a point.
					return new PathState(i, currentTime - (long)((distance - distancedTraveledSince) / movementSpeedPerSecond) * TimeSpan.TicksPerMillisecond);
				}
			}

			return new PathState(MovementData.MovementPath.Count - 1, 0);
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//We don't need to do anything in update, position is already changed in start.

			//TODO: Implement incremental Update, calling start is SLOW and a hack, until implemented
			Start(entity, currentTime);
		}
	}
}
