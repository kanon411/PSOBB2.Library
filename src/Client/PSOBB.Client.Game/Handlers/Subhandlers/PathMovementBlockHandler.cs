using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PSOBB
{
	public sealed class PathMovementBlockHandler : SpecificMovementTypeHandler<PathBasedMovementData>
	{
		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		private IEntityGuidMappable<IMovementGenerator<GameObject>> MovementGenerator { get; }

		private IEntityGuidMappable<GameObject> GameObjectMap { get; }

		private INetworkTimeService TimeService { get; }

		/// <inheritdoc />
		public PathMovementBlockHandler(
			IEntityGuidMappable<IMovementData> movementDataMap,
			IEntityGuidMappable<IMovementGenerator<GameObject>> movementGenerator,
			IEntityGuidMappable<GameObject> gameObjectMap,
			INetworkTimeService timeService)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			MovementGenerator = movementGenerator ?? throw new ArgumentNullException(nameof(movementGenerator));
			GameObjectMap = gameObjectMap ?? throw new ArgumentNullException(nameof(gameObjectMap));
			TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
		}

		/// <inheritdoc />
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PathBasedMovementData data)
		{
			MovementDataMap[entityGuid] = data;
			MovementGenerator[entityGuid] = new PathMovementGenerator(data);
		}
	}
}
