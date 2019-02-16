using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	//TODO: This is mostly copy-paste from the server one. Can we unify this somehow?
	public sealed class PositionChangeMovementGenerator : BaseMovementGenerator<PositionChangeMovementData>
	{
		//This is actually unity stuff, for various reasons I have opted to directly depend on it.
		private CharacterController Controller { get; }

		private Vector3 CachedMovementDirection;

		//TODO: We shouldn't do this here
		private float DefaultPlayerSpeed = 5.0f;

		/// <inheritdoc />
		public PositionChangeMovementGenerator(PositionChangeMovementData movementData, [NotNull] CharacterController controller) 
			: base(movementData)
		{
			Controller = controller ?? throw new ArgumentNullException(nameof(controller));
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			//We don't need to deal with time when a position change occurs.

			//TODO: This is demo code, we should handle actual movement differently.
			entity.transform.position = MovementData.InitialPosition;

			//Now, we should also create the movement direction
			CachedMovementDirection = new Vector3(MovementData.Direction.x, 0.0f, MovementData.Direction.y);

			CachedMovementDirection = entity.transform.TransformDirection(CachedMovementDirection);
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//TODO: We should have real handling at some point.
			float diff = DiffFromStartTime(currentTime);

			//gravity
			//Don't need to subtract the cached direction Y because it should be 0, or treated as 0.
			CachedMovementDirection.y = (9.8f * diff);
			Controller.Move(CachedMovementDirection * diff);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float DiffFromStartTime(long currentTime)
		{
			return (float)(currentTime - MovementData.TimeStamp) / TimeSpan.TicksPerMillisecond;
		}
	}
}
