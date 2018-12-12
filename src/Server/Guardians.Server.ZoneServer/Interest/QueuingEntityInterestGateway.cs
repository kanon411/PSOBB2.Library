using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class QueuingEntityInterestGateway : IInterestRadiusManager
	{
		private EntityInterestChangeQueue ChangeQueue { get; }

		/// <inheritdoc />
		public QueuingEntityInterestGateway([NotNull] EntityInterestChangeQueue changeQueue)
		{
			ChangeQueue = changeQueue ?? throw new ArgumentNullException(nameof(changeQueue));
		}

		/// <inheritdoc />
		public bool TryEntityEnter(NetworkEntityGuid entryContext, NetworkEntityGuid entityGuid)
		{
			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Enter));

			return true;
		}

		/// <inheritdoc />
		public bool TryEntityLeave(NetworkEntityGuid entryContext, NetworkEntityGuid entityGuid)
		{
			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Exit));

			return true;
		}
	}
}
