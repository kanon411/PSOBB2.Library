﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;

namespace PSOBB
{
	public sealed class DefaultEntityCreationContext : IEntityCreationContext
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public IMovementData MovementData { get; }

		/// <inheritdoc />
		public EntityPrefab PrefabType { get; }

		/// <inheritdoc />
		public IEntityDataFieldContainer EntityData { get; }

		/// <inheritdoc />
		public DefaultEntityCreationContext(NetworkEntityGuid entityGuid, IMovementData movementData, EntityPrefab prefabType, [NotNull] IEntityDataFieldContainer entityData)
		{
			if(!Enum.IsDefined(typeof(EntityPrefab), prefabType)) throw new InvalidEnumArgumentException(nameof(prefabType), (int)prefabType, typeof(EntityPrefab));
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
			PrefabType = prefabType;
			EntityData = entityData ?? throw new ArgumentNullException(nameof(entityData));
		}
	}
}
