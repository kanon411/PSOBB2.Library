using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
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
		public NetworkEntityGuid EntityGuid { get; }

		/// <summary>
		/// The creation data for the entity.
		/// </summary>
		public EntityCreationData CreationData { get; }

		/// <inheritdoc />
		public NetworkEntityNowVisibleEventArgs([NotNull] NetworkEntityGuid entityGuid, [NotNull] EntityCreationData creationData)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			CreationData = creationData ?? throw new ArgumentNullException(nameof(creationData));
		}
	}
}
