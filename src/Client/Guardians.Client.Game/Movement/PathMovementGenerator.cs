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

			/// <summary>
			/// This is different than the movement data timestamp.
			/// This is the time that the simulation started this particiular path state.
			/// Meaning it can be used to compute the offset since the last path state
			/// which would can then be used to compute the distance traveled since the path at <see cref="PathIndex"/>.
			/// How it relates to <see cref="IndexTimeOffset"/> is that it IndexTimeOffset is like P0 in
			/// FinalPosition = Speed * t + P0 where P0 is <see cref="IndexTimeOffset"/> * Speed
			/// </summary>
			public long TimePathStateCreated { get; }

			/// <summary>
			/// The duration (in ticks) for how long it should take from <see cref="PathIndex"/> to <see cref="PathIndex"/>+1.
			/// This includes the <see cref="IndexTimeOffset"/>.
			/// Meaning at t = <see cref="TimePathStateCreated"/> + <see cref="PathSegementDuration"/> the path should move to the next segement.
			/// </summary>
			public long PathSegementDuration { get; }

			/// <inheritdoc />
			public PathState(int pathIndex, long indexTimeOffset, long timePathStateCreated, long pathSegementDuration)
			{
				//Path can be 0, it can be the initial segment
				if(pathIndex < 0) throw new ArgumentOutOfRangeException(nameof(pathIndex));
				if(indexTimeOffset < 0) throw new ArgumentOutOfRangeException(nameof(indexTimeOffset));
				if(timePathStateCreated < 0) throw new ArgumentOutOfRangeException(nameof(timePathStateCreated));
				if(pathSegementDuration < 0) throw new ArgumentOutOfRangeException(nameof(pathSegementDuration));

				PathIndex = pathIndex;
				IndexTimeOffset = indexTimeOffset;
				TimePathStateCreated = timePathStateCreated;
				PathSegementDuration = pathSegementDuration;
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

			//Just call update, which will set the position.
			InternalUpdate(entity, currentTime);
		}

		private PathState ComputeInitialPathState(long currentTime)
		{
			//So when a path starts, it may not be on the "initial position" because it could have
			//started pathing long before this movement data was created on the server
			//Therefore we must step forward out simulation to the point where it should be.
			long startTimeDiff = Math.Max(currentTime - this.MovementData.TimeStamp, 0); //we don't want to deal with negative time diff
			float movementSpeedPerSecond = 1.0f;

			//TODO: Handle case where the start time is actually the timestamp (unrealistic real world)

			//TODO: How should we deal with a negative time diff??
			if(startTimeDiff < 0)
				throw new InvalidOperationException($"Encountered Negative Time Diff. CurrentTime: {currentTime} MovementStamp: {MovementData.TimeStamp}");

			for(int i = 0; i < this.MovementData.MovementPath.Count - 1; i++)
			{
				Vector3 distanceVectorOffset = ComputeDistanceOffsetByMovementDataIndex(i);

				//Compute how long it would take to travel this distance at the current speed
				//TODO: Handle different speeds
				float distance = distanceVectorOffset.magnitude;

				//This is the time it would have traveled since the timestamp
				float distancedTraveledSince = (startTimeDiff / TimeSpan.TicksPerMillisecond) * (movementSpeedPerSecond / 1000f);

				if(distancedTraveledSince > distance)
				{
					//this means that the time elasped would put us past the initial path point
					//so we must remove the time taken to travel the the initial ray and continue down the path
					startTimeDiff -= (long)((distance) / (movementSpeedPerSecond)) * 1000 * TimeSpan.TicksPerMillisecond; // (x / speed) = time
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
					return new PathState(i, CalculateInitialTickOffsetForPathState(movementSpeedPerSecond, distance, distancedTraveledSince), currentTime, CalculateDistanceLengthInTicks(distance, movementSpeedPerSecond)); // * 1000 to convert secounds to milliseconds
				}
			}

			return new PathState(MovementData.MovementPath.Count - 1, 0, currentTime, 0);
		}

		private static long CalculateDistanceLengthInTicks(float distance, float movementSpeedInSeconds)
		{
			//(distance / movementSpeedInSeconds) = seconds taken to travel distance.
			// * 1000 converts to milliseconds
			// * TicksPerMillisecond converts it to ticks.
			return ((long)(distance / movementSpeedInSeconds) * 1000 * TimeSpan.TicksPerMillisecond);
		}

		private Vector3 ComputeDistanceOffsetByMovementDataIndex(int i)
		{
			//We need B - A to find direction from A to B, made a mistake earlier with vector math and did A - B. That was wrong
			return this.MovementData.MovementPath[i] - this.MovementData.MovementPath[i + 1];
		}

		private static long CalculateInitialTickOffsetForPathState(float movementSpeedPerSecond, float distance, float distancedTraveledSince)
		{
			//This computes how many ticks initially passed since the path segement started
			//distance (total) - distanceTraveled = displacement
			return CalculateDistanceLengthInTicks(distance - distancedTraveledSince, movementSpeedPerSecond);
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//Knowing the two points is enough to linerally interpolate between them.
			long timeSinceSegementState = currentTime - State.TimePathStateCreated;
			float lerpRatio = (float)timeSinceSegementState / State.PathSegementDuration;
			entity.transform.position = Vector3.Lerp(MovementData.MovementPath[State.PathIndex], MovementData.MovementPath[State.PathIndex + 1], lerpRatio);

			if(lerpRatio >= 1.0f)
				State = new PathState(State.PathIndex + 1, 0, currentTime, CalculateDistanceLengthInTicks(ComputeDistanceOffsetByMovementDataIndex(State.PathIndex + 1).magnitude, 1.0f));
		}
	}
}
