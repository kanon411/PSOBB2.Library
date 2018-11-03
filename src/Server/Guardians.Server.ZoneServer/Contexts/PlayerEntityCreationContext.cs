using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class PlayerEntityCreationContext : IEntityGuidContainer, IEntityCreationContext
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public PlayerEntitySessionContext SessionContext { get; }

		/// <inheritdoc />
		public IMovementData MovementData { get; }

		/// <inheritdoc />
		public EntityPrefab PrefabType { get; }

		/// <inheritdoc />
		public PlayerEntityCreationContext([NotNull] NetworkEntityGuid entityGuid, [NotNull] PlayerEntitySessionContext sessionContext, [NotNull] IMovementData movementData, EntityPrefab prefabType)
		{
			if(!Enum.IsDefined(typeof(EntityPrefab), prefabType)) throw new InvalidEnumArgumentException(nameof(prefabType), (int)prefabType, typeof(EntityPrefab));
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			SessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
			PrefabType = prefabType;
		}

		protected PlayerEntityCreationContext()
		{
			
		}
	}
}
