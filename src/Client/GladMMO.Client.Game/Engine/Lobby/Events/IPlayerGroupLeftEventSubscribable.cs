using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface IPlayerGroupLeftEventSubscribable
	{
		event EventHandler<PlayerLeftGroupEventArgs> OnPlayerLeftGroup;
	}

	public sealed class PlayerLeftGroupEventArgs : EventArgs
	{
		public ObjectGuid PlayerGuid { get; }

		/// <inheritdoc />
		public PlayerLeftGroupEventArgs([NotNull] ObjectGuid playerGuid)
		{
			PlayerGuid = playerGuid ?? throw new ArgumentNullException(nameof(playerGuid));
		}
	}
}
