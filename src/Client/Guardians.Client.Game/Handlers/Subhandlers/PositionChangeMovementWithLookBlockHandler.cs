using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Guardians
{
	public sealed class PositionChangeMovementWithLookBlockHandler : SpecificMovementTypeHandler<PositionChangeMovementDataWithLook>
	{
		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		private IEntityGuidMappable<IMovementGenerator<GameObject>> MovementGenerator { get; }

		private IEntityGuidMappable<GameObject> GameObjectMap { get; }

		private INetworkTimeService TimeService { get; }

		/// <inheritdoc />
		public PositionChangeMovementWithLookBlockHandler(
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
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PositionChangeMovementDataWithLook data)
		{
			MovementDataMap[entityGuid] = data;
			MovementGenerator[entityGuid] = new ClientPositionChangeWithLookMovementGenerator(data);

			//TODO: We may not be on the main thread, so we might not be able to do this.
			MovementGenerator[entityGuid].Update(GameObjectMap[entityGuid], TimeService.CurrentRemoteTime);
		}
	}
}
