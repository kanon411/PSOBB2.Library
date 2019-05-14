using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface INetworkEntityVisibleEventSubscribable
	{
		event EventHandler<NetworkEntityNowVisibleEventArgs> OnNetworkEntityNowVisible;
	}

	public sealed class NetworkEntityNowVisibleEventArgs : EventArgs, IEntityGuidContainer
	{
		//I know EntityGuid is apart of CreationData but it probbaly won't be for too much longer.
		/// <summary>
		/// The entity guid.
		/// </summary>
		public ObjectGuid EntityGuid { get; }

		/// <inheritdoc />
		public NetworkEntityNowVisibleEventArgs([NotNull] ObjectGuid entityGuid)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
		}
	}
}
