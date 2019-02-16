using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public sealed class ServerPlayerInputChangeMovementGenerator : LateInitializationBaseMovementGenerator<PositionChangeMovementData>
	{
		private Vector2 Input { get; }

		public Action<PositionChangeMovementData> OnCreatedCallback { get; }

		//This is actually unity stuff, for various reasons I have opted to directly depend on it.
		private CharacterController Controller { get; }

		private Vector3 CachedMovementDirection;

		//TODO: We shouldn't do this here
		private float DefaultPlayerSpeed = 5.0f;

		/// <inheritdoc />
		public ServerPlayerInputChangeMovementGenerator(Vector2 input, Action<PositionChangeMovementData> onCreatedCallback, [NotNull] CharacterController controller)
		{
			Input = input.normalized;
			OnCreatedCallback = onCreatedCallback;
			Controller = controller ?? throw new ArgumentNullException(nameof(controller));
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			base.Start(entity, currentTime);

			//At this point, movement data is initialized so we should set the movement data.
			OnCreatedCallback?.Invoke(MovementData);

			//Now, we should also create the movement direction
			CachedMovementDirection = new Vector3(MovementData.Direction.x, 0.0f, MovementData.Direction.y);

			CachedMovementDirection = entity.transform.TransformDirection(CachedMovementDirection);
		}

		/// <inheritdoc />
		protected override PositionChangeMovementData InitializeMovementData(GameObject entity, long currentTime)
		{
			return new PositionChangeMovementData(currentTime, Input, entity.transform.position);
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