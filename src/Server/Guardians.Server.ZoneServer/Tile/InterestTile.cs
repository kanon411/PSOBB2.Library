using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class InterestTile
	{
		private readonly List<NetworkEntityGuid> _ContainedEntities = new List<NetworkEntityGuid>();

		/// <summary>
		/// Represents the contained entites.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> ContainedEntities => _ContainedEntities;

		/// <summary>
		/// The internally managed queue that contains all the entites that are leaving the queue
		/// in the next update.
		/// </summary>
		private Queue<NetworkEntityGuid> LeavingTileQueue = new Queue<NetworkEntityGuid>();

		/// <summary>
		/// Internally managed queue that contains all the enties that are entering the tile
		/// in the next update.
		/// </summary>
		private Queue<NetworkEntityGuid> EnteringTileQueue = new Queue<NetworkEntityGuid>();

		/// <summary>
		/// The collection of enties that are queued for leaving the tile.
		/// They have not left the title if they are in this collection, so won't be in <see cref="ContainedEntities"/>.
		/// They will leave the title in the next update.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> QueuedLeavingEntities => LeavingTileQueue;

		/// <summary>
		/// The collection of enties that are queued for entering the tile.
		/// They have not actually joined the tile so won't be in <see cref="ContainedEntities"/>.
		/// They will join the tile in the next update.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> QueueJoiningEntities => EnteringTileQueue;
	}
}
