using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class QueueBasedPlayerEntitySessionGateway : IEntityGateway<PlayerEntitySessionContext>, IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>>
	{
		private readonly object SyncObj = new object();

		private Queue<KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>> PlayerCreateQueue { get; } = new Queue<KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>>();

		/// <inheritdoc />
		public bool isEmpty
		{
			get
			{
				lock(SyncObj)
					return PlayerCreateQueue.Count == 0;
			}
		}

		/// <inheritdoc />
		public bool TryEntityEnter(PlayerEntitySessionContext entryContext, NetworkEntityGuid entityGuid)
		{
			lock(SyncObj)
				PlayerCreateQueue.Enqueue(new KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>(entityGuid, entryContext));

			return true;
		}

		/// <inheritdoc />
		public bool TryEntityLeave(PlayerEntitySessionContext entryContext, NetworkEntityGuid entityGuid)
		{
			throw new NotImplementedException($"TODO: This is an unsupported action. Not even sure how we should implement this.");
		}

		/// <inheritdoc />
		public KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext> Dequeue()
		{
			lock(SyncObj)
				return PlayerCreateQueue.Dequeue();
		}
	}
}
