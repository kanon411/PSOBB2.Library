using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class QueuingEntityInterestGateway : IInterestRadiusManager
	{
		private EntityInterestChangeQueue ChangeQueue { get; }

		private IReadonlyEntityGuidMappable<InterestCollection> ManagedInterestCollections { get; }

		/// <inheritdoc />
		public QueuingEntityInterestGateway([NotNull] EntityInterestChangeQueue changeQueue, [NotNull] IReadonlyEntityGuidMappable<InterestCollection> managedInterestCollections)
		{
			ChangeQueue = changeQueue ?? throw new ArgumentNullException(nameof(changeQueue));
			ManagedInterestCollections = managedInterestCollections ?? throw new ArgumentNullException(nameof(managedInterestCollections));
		}
		
		/// <inheritdoc />
		public bool TryEntityEnter([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			//We don't need to lock because this always runs on the main thread, and writes to interest collection always happen on the main thread.
			//If it's already known then we should ignore it.
			if(ManagedInterestCollections[entryContext].Contains(entityGuid))
				return false;

			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Enter));

			return true;
		}

		/// <inheritdoc />
		public bool TryEntityLeave([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			//We don't need to lock because this always runs on the main thread, and writes to interest collection always happen on the main thread.
			//If we don't know the entity, then we should not queue up a removal.
			if(!ManagedInterestCollections[entryContext].Contains(entityGuid))
				return false;

			ChangeQueue.Enqueue(new EntityInterestChangeContext(entryContext, entityGuid, EntityInterestChangeContext.ChangeType.Exit));

			return true;
		}
	}
}
