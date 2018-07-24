using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class PlayerEntityCreationContext : IEntityGuidContainer
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public PlayerEntitySessionContext SessionContext { get; }

		/// <inheritdoc />
		public PlayerEntityCreationContext([NotNull] NetworkEntityGuid entityGuid, [NotNull] PlayerEntitySessionContext sessionContext)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			SessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
		}

		protected PlayerEntityCreationContext()
		{
			
		}
	}
}
