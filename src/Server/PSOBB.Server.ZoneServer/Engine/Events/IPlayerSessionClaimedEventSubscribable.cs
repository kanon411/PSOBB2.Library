using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public interface IPlayerSessionClaimedEventSubscribable
	{
		event EventHandler<PlayerSessionClaimedEventArgs> OnSuccessfulSessionClaimed;
	}

	public sealed class PlayerSessionClaimedEventArgs : EventArgs
	{
		public NetworkEntityGuid EntityGuid { get; }

		public Vector3 SpawnPosition { get; }

		/// <summary>
		/// The connection ID of the session.
		/// </summary>
		public int ConnectionId { get; }

		/// <inheritdoc />
		public PlayerSessionClaimedEventArgs([NotNull] NetworkEntityGuid entityGuid, Vector3 spawnPosition, int connectionId)
		{
			if(connectionId < 0) throw new ArgumentOutOfRangeException(nameof(connectionId));

			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			SpawnPosition = spawnPosition;
			ConnectionId = connectionId;
		}
	}
}
