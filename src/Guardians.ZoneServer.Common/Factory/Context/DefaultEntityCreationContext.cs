using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class DefaultEntityCreationContext : IEntityCreationContext
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public MovementInformation MovementData { get; }

		/// <inheritdoc />
		public EntityPrefab PrefabType { get; }

		/// <inheritdoc />
		public DefaultEntityCreationContext(NetworkEntityGuid entityGuid, MovementInformation movementData, EntityPrefab prefabType)
		{
			if(!Enum.IsDefined(typeof(EntityPrefab), prefabType)) throw new InvalidEnumArgumentException(nameof(prefabType), (int)prefabType, typeof(EntityPrefab));
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
			PrefabType = prefabType;
		}
	}
}
