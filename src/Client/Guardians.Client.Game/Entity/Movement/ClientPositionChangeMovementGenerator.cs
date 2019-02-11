using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Guardians
{
	public sealed class ClientPositionChangeMovementGenerator : BaseMovementGenerator<PositionChangeMovementData>
	{
		private Vector3 ComputedNormalizedMovementDirection;

		private float lastHitHeight;

		/// <inheritdoc />
		public ClientPositionChangeMovementGenerator(PositionChangeMovementData movementData) 
			: base(movementData)
		{
			
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			//We don't need to deal with time when a position change occurs.

			//TODO: Reimplement entity rotation direction
			//entity.transform.SetPositionAndRotation(MovementData.InitialPosition, Quaternion.Euler(Vector3.up * MovementData.YAxisRotation));

			ComputedNormalizedMovementDirection = (entity.transform.right * MovementData.Direction.x + entity.transform.forward * MovementData.Direction.y);

			//Remove any potential Y value for safety
			ComputedNormalizedMovementDirection = new Vector3(ComputedNormalizedMovementDirection.x, 0, ComputedNormalizedMovementDirection.z).normalized;
		}

		//TODO: This is a strange mess.
		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//TODO: This is likely to be SLOW and costly. Can we avoid this?
			NavMesh.SamplePosition(entity.transform.position, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas);
			if(navHit.hit)
			{
				entity.transform.position = navHit.position;
				//Kinda a hack, but let's set the Y of the initialpos so that it stays on the navmesh while lerping
				entity.transform.position.Set(entity.transform.position.x, navHit.position.y, entity.transform.position.z);
				lastHitHeight = navHit.position.y;
			}

			//TODO: We are harcoding speed here, we shouldn't do that.
			ProjectVersionStage.AssertAlpha();
			//TODO: The time syncronization is not working, it's off by like 0.15 seconds for some reason.
			//entity.transform.position = MovementData.InitialPosition + ComputedNormalizedMovementDirection * ComputeTimestampDiffSeconds(currentTime) * 3.0f;
			entity.transform.position = MovementData.InitialPosition + (ComputedNormalizedMovementDirection * ComputeTimestampDiffSeconds(currentTime) * 3.0f);

			entity.transform.position = new Vector3(entity.transform.position.x, lastHitHeight, entity.transform.position.z);
		}

		private float ComputeTimestampDiffSeconds(long currentTime)
		{
			long diff = currentTime - MovementData.TimeStamp;

			//This will actually happen right now on intial position, due to spoofed time from the server
			//if(diff < 0)
			//	Debug.LogError($"Diff Less Than Zero: {diff}");

			//Must use ticks per second as time.deltaTime is in seconds
			return (float)(diff < 0 ? 0 : diff) / (float)TimeSpan.TicksPerSecond;
		}
	}
}
