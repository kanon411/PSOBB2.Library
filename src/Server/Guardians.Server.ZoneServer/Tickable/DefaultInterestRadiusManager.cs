using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class DefaultInterestRadiusManager : IInterestRadiusManager, IGameTickable
	{
		private IReadonlyEntityGuidMappable<InterestCollection> ManagedInterestCollections { get; }

		private INetworkMessageSender<EntityVisibilityChangeContext> VisibilityMessageSender { get; }

		/// <inheritdoc />
		public DefaultInterestRadiusManager(
			[NotNull] IReadonlyEntityGuidMappable<InterestCollection> managedInterestCollections,
			[NotNull] VisibilityChangeMessageSender visibilityMessageSender)
		{
			ManagedInterestCollections = managedInterestCollections ?? throw new ArgumentNullException(nameof(managedInterestCollections));
			VisibilityMessageSender = visibilityMessageSender ?? throw new ArgumentNullException(nameof(visibilityMessageSender));
		}

		/// <inheritdoc />
		public bool TryEntityEnter([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			ThrowIfNoEntityInterestManaged(entryContext, entityGuid);

			//If it's already known then we should ignore it.
			if(ManagedInterestCollections[entryContext].Contains(entityGuid))
				return false;

			ManagedInterestCollections[entryContext].Register(entityGuid, entityGuid);

			return true;
		}

		private void ThrowIfNoEntityInterestManaged(NetworkEntityGuid entryContext, NetworkEntityGuid entityGuid)
		{
			if(!ManagedInterestCollections.ContainsKey(entryContext))
				throw new InvalidOperationException($"Guid: {entityGuid} tried to enter Entity: {entryContext} interest. But Entity does not maintain interest. Does not exist in interest collection.");
		}

		/// <inheritdoc />
		public bool TryEntityLeave([NotNull] NetworkEntityGuid entryContext, [NotNull] NetworkEntityGuid entityGuid)
		{
			if(entryContext == null) throw new ArgumentNullException(nameof(entryContext));
			if(entityGuid == null) throw new ArgumentNullException(nameof(entityGuid));

			ThrowIfNoEntityInterestManaged(entryContext, entityGuid);

			//TODO: Handle case where it's not known and we want to unknow it somehow

			return ManagedInterestCollections[entryContext].Unregister(entityGuid);
		}

		/// <inheritdoc />
		public void Tick()
		{
			//TODO: This should probably run AFTER we broadcast movement data
			
			//We need to iterate the entire interest dictionary
			//That means we need to check the new incoming and outgoing entities
			//We do this because we need to build update packets for the players
			//so that they can become aware of them AND we can start pushing
			//events to them
			foreach(var kvp in ManagedInterestCollections)
			{
				//We want to skip any collection that doesn't have any pending changes.
				//No reason to send a message about it nor dequeue anything
				if(!kvp.Value.HasPendingChanges())
					continue;

				//We should only build packets for players.
				if(kvp.Key.EntityType == EntityType.Player)
				{
					VisibilityMessageSender.Send(new EntityVisibilityChangeContext(kvp.Key, kvp.Value));
				}

				//No matter player or NPC we should dequeue the joining/leaving
				//entites so that the state of the known entites reflects the diff packets sent
				InterestDequeueSetCommand dequeueCommand = new InterestDequeueSetCommand(kvp.Value, kvp.Value);

				//TODO: Should we execute right away? Or after all packets are sent?
				dequeueCommand.Execute();
			}

#if DEBUG || DEBUG_BUILD
			foreach(var kvp in ManagedInterestCollections)
			{
				if(!kvp.Value.EnteringDequeueable.isEmpty)
					throw new InvalidOperationException($"Failed to fully queue: {nameof(kvp.Value.EnteringDequeueable)}");

				if(!kvp.Value.LeavingDequeueable.isEmpty)
					throw new InvalidOperationException($"Failed to fully queue: {nameof(kvp.Value.LeavingDequeueable)}");
			}
#endif
		}
	}
}
