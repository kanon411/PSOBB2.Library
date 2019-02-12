using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class QueueBasedPlayerEntitySessionGateway : IEntityGateway<PlayerEntityEnterWorldCreationContext>, IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>>
	{
		private readonly object SyncObj = new object();

		private Queue<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>> PlayerCreateQueue { get; } = new Queue<KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>>();

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
		public bool TryEntityEnter(PlayerEntityEnterWorldCreationContext entryContext, NetworkEntityGuid entityGuid)
		{
			lock(SyncObj)
				PlayerCreateQueue.Enqueue(new KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext>(entityGuid, entryContext));

			return true;
		}

		/// <inheritdoc />
		public bool TryEntityLeave(PlayerEntityEnterWorldCreationContext entryContext, NetworkEntityGuid entityGuid)
		{
			throw new NotImplementedException($"TODO: This is an unsupported action. Not even sure how we should implement this.");
		}

		/// <inheritdoc />
		public KeyValuePair<NetworkEntityGuid, PlayerEntityEnterWorldCreationContext> Dequeue()
		{
			lock(SyncObj)
				return PlayerCreateQueue.Dequeue();
		}
	}
}
