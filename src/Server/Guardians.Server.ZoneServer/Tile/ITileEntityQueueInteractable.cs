namespace Guardians
{
	public interface ITileEntityQueueInteractable
	{
		/// <summary>
		/// The internally managed queue that contains all the entites that are leaving the queue
		/// in the next update.
		/// </summary>
		IDequeable<NetworkEntityGuid> LeavingTileQueue { get; }

		/// <summary>
		/// Internally managed queue that contains all the enties that are entering the tile
		/// in the next update.
		/// </summary>
		IDequeable<NetworkEntityGuid> EnteringTileQueue { get; }
	}
}