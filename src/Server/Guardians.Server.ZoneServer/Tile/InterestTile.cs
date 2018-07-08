using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	
	/// <summary>
	/// Add/Remove interface for an interest set.
	/// (We don't use ICollection because of all the ridiculous methods like CopyTo and others)
	/// </summary>
	public interface ITileEntityInterestSet
	{
		/// <summary>
		/// Adds an entity to the interest set.
		/// </summary>
		/// <param name="guid">The entity to add.</param>
		/// <returns>True if the entity was added. False if it is already contained.</returns>
		bool Add(NetworkEntityGuid guid);

		/// <summary>
		/// Tries to remove an entity in the interest set.
		/// If it is in the interest set, and successfully removed, it will return true.
		/// If it not contained it will return false.
		/// </summary>
		/// <param name="guid">The entity to add.</param>
		/// <returns>True if the entity was found and removed.</returns>
		bool Remove(NetworkEntityGuid guid);
	}

	public sealed class InterestTile : IReadonlyInterestTile, ITileEntityInterestQueueable, ITileEntityInterestDequeueable, ITileEntityInterestSet
	{
		/// <inheritdoc />
		public int TileId { get; }

		/// <summary>
		/// Set that contains all the entites in the tile.
		/// Is a unique set.
		/// </summary>
		private readonly HashSet<NetworkEntityGuid> _ContainedEntities = new HashSet<NetworkEntityGuid>();

		/// <summary>
		/// Ordered queue of leaving entites.
		/// Should be contained in <see cref="ContainedEntities"/>
		/// </summary>
		private readonly TileEntityQueue _LeavingTileQueue = new TileEntityQueue();

		/// <summary>
		/// Ordered queue of entering entites.
		/// Not contained in <see cref="ContainedEntities"/>
		/// </summary>
		private readonly TileEntityQueue _EnteringTileQueue = new TileEntityQueue();

		/// <summary>
		/// Represents the contained entites.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> ContainedEntities => _ContainedEntities;

		/// <summary>
		/// The collection of enties that are queued for leaving the tile.
		/// They have not left the title if they are in this collection, so won't be in <see cref="ContainedEntities"/>.
		/// They will leave the title in the next update.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> QueuedLeavingEntities => _LeavingTileQueue;

		/// <summary>
		/// The collection of enties that are queued for entering the tile.
		/// They have not actually joined the tile so won't be in <see cref="ContainedEntities"/>.
		/// They will join the tile in the next update.
		/// </summary>
		public IReadOnlyCollection<NetworkEntityGuid> QueueJoiningEntities => _EnteringTileQueue;

		/// <inheritdoc />
		public IDequeable<NetworkEntityGuid> LeavingTileQueue => _LeavingTileQueue;

		/// <inheritdoc />
		public IDequeable<NetworkEntityGuid> EnteringTileQueue => _EnteringTileQueue;

		/// <inheritdoc />
		public InterestTile(int tileId)
		{
			TileId = tileId;
		}

		/// <inheritdoc />
		public void Register([NotNull] NetworkEntityGuid key, [NotNull] NetworkEntityGuid value)
		{
			if(key == null) throw new ArgumentNullException(nameof(key));
			if(value == null) throw new ArgumentNullException(nameof(value));

			//Both key and value are the same
			_EnteringTileQueue.Enqueue(value);
		}

		/// <inheritdoc />
		public bool Contains([NotNull] NetworkEntityGuid key)
		{
			if(key == null) throw new ArgumentNullException(nameof(key));

			return _ContainedEntities.Contains(key);
		}

		/// <inheritdoc />
		public NetworkEntityGuid Retrieve(NetworkEntityGuid key)
		{
			if(!_ContainedEntities.Contains(key))
				throw new InvalidOperationException($"Provided Key: {key} does not exist in the tile.");

			return key;
		}

		/// <inheritdoc />
		public bool Unregister([NotNull] NetworkEntityGuid key)
		{
			if(key == null) throw new ArgumentNullException(nameof(key));

			_LeavingTileQueue.Enqueue(key);
			return true;
		}

		/// <inheritdoc />
		public bool Add([NotNull] NetworkEntityGuid guid)
		{
			if(guid == null) throw new ArgumentNullException(nameof(guid));

			return _ContainedEntities.Add(guid);
		}

		/// <inheritdoc />
		public bool Remove([NotNull] NetworkEntityGuid guid)
		{
			if(guid == null) throw new ArgumentNullException(nameof(guid));

			return _ContainedEntities.Remove(guid);
		}
	}
}
