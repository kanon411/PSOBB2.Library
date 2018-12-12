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
		public bool TryEntityEnter([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Enter));

			return true;
		}

		/// <inheritdoc />
		public bool TryEntityLeave([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Exit));

			return true;
		}
	}
}
