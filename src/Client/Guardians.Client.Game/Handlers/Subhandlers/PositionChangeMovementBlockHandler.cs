using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Guardians
{
	public sealed class PositionChangeMovementBlockHandler : SpecificMovementTypeHandler<PositionChangeMovementData>
	{
		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		private IEntityGuidMappable<IMovementGenerator<GameObject>> MovementGenerator { get; }

		/// <inheritdoc />
		public PositionChangeMovementBlockHandler(
			IEntityGuidMappable<IMovementData> movementDataMap,
			IEntityGuidMappable<IMovementGenerator<GameObject>> movementGenerator)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			MovementGenerator = movementGenerator ?? throw new ArgumentNullException(nameof(movementGenerator));
		}

		/// <inheritdoc />
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PositionChangeMovementData data)
		{
			MovementDataMap[entityGuid] = data;
			MovementGenerator[entityGuid] = new PositionChangeMovementGenerator(data);
		}
	}
}
