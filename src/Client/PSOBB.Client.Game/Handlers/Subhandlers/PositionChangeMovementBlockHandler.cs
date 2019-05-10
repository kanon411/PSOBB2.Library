using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GladMMO
{
	public sealed class PositionChangeMovementBlockHandler : SpecificMovementTypeHandler<PositionChangeMovementData>
	{
		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		private IEntityGuidMappable<IMovementGenerator<GameObject>> MovementGenerator { get; }

		private IEntityGuidMappable<GameObject> GameObjectMap { get; }

		private INetworkTimeService TimeService { get; }

		private IReadonlyEntityGuidMappable<CharacterController> CharacterControllerMappable { get; }

		/// <inheritdoc />
		public PositionChangeMovementBlockHandler(
			IEntityGuidMappable<IMovementData> movementDataMap,
			IEntityGuidMappable<IMovementGenerator<GameObject>> movementGenerator,
			IEntityGuidMappable<GameObject> gameObjectMap,
			INetworkTimeService timeService, 
			[NotNull] IReadonlyEntityGuidMappable<CharacterController> characterControllerMappable)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			MovementGenerator = movementGenerator ?? throw new ArgumentNullException(nameof(movementGenerator));
			GameObjectMap = gameObjectMap ?? throw new ArgumentNullException(nameof(gameObjectMap));
			TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
			CharacterControllerMappable = characterControllerMappable ?? throw new ArgumentNullException(nameof(characterControllerMappable));
		}

		/// <inheritdoc />
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PositionChangeMovementData data)
		{
			//TODO: Only works for players.
			MovementDataMap[entityGuid] = data;
			MovementGenerator[entityGuid] = new PositionChangeMovementGenerator(data, CharacterControllerMappable[entityGuid]);
		}
	}
}
