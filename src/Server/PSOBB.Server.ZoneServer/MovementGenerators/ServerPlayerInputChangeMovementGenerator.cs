using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public sealed class ServerPlayerInputChangeMovementGenerator : LateInitializationBaseMovementGenerator<PositionChangeMovementData>
	{
		private Vector2 Input { get; }

		public Action<PositionChangeMovementData> OnCreatedCallback { get; }

		/// <inheritdoc />
		public ServerPlayerInputChangeMovementGenerator(Vector2 input, Action<PositionChangeMovementData> onCreatedCallback)
		{
			Input = input;
			OnCreatedCallback = onCreatedCallback;
		}

		/// <inheritdoc />
		protected override void Start(GameObject entity, long currentTime)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			base.Start(entity, currentTime);

			//At this point, movement data is initialized so we should set the movement data.
			OnCreatedCallback?.Invoke(MovementData);
		}

		/// <inheritdoc />
		protected override PositionChangeMovementData InitializeMovementData(GameObject entity, long currentTime)
		{
			return new PositionChangeMovementData(currentTime, Input, entity.transform.position);
		}

		/// <inheritdoc />
		protected override void InternalUpdate(GameObject entity, long currentTime)
		{
			//We don't need to do anything in update, position is already changed in start.
		}
	}
}