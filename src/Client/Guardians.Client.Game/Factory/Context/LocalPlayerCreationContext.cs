using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class LocalPlayerCreationContext : IEntityGuidContainer
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public MovementInformation MovementData { get; }

		/// <inheritdoc />
		public LocalPlayerCreationContext(NetworkEntityGuid entityGuid, MovementInformation movementData)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
		}
	}
}
