using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IPlayerWorldSessionCreatedEventSubscribable
	{
		event EventHandler<PlayerWorldSessionCreationEventArgs> OnPlayerWorldSessionCreated;
	}

	public sealed class PlayerWorldSessionCreationEventArgs : EventArgs
	{
		public NetworkEntityGuid EntityGuid { get; }

		/// <inheritdoc />
		public PlayerWorldSessionCreationEventArgs([NotNull] NetworkEntityGuid entityGuid)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
		}
	}
}
