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

		private IReadonlyEntityGuidMappable<GameObject> GameObjectMap { get; }

		private IReadonlyEntityGuidMappable<CharacterController> CharacterControllerMappable { get; }

		/// <inheritdoc />
		public PositionChangeMovementBlockHandler(
			IEntityGuidMappable<IMovementData> movementDataMap,
			IEntityGuidMappable<IMovementGenerator<GameObject>> movementGenerator,
			IReadonlyEntityGuidMappable<GameObject> gameObjectMap, 
			[NotNull] IReadonlyEntityGuidMappable<CharacterController> characterControllerMappable)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			MovementGenerator = movementGenerator ?? throw new ArgumentNullException(nameof(movementGenerator));
			GameObjectMap = gameObjectMap ?? throw new ArgumentNullException(nameof(gameObjectMap));
			CharacterControllerMappable = characterControllerMappable ?? throw new ArgumentNullException(nameof(characterControllerMappable));
		}

		/// <inheritdoc />
		protected override void HandleMovement(NetworkEntityGuid entityGuid, PositionChangeMovementData data)
		{
			MovementDataMap[entityGuid] = data;
			MovementGenerator[entityGuid] = new PositionChangeMovementGenerator(data, CharacterControllerMappable[entityGuid]);
		}
	}
}
