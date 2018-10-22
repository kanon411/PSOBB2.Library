using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class MovementUpdateMessageSender : INetworkMessageSender<EntityMovementMessageContext>
	{
		private IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> SessionMappable { get; }

		private IReadonlyEntityGuidMappable<InterestCollection> GuidToInterestCollectionMappable { get; }

		private IReadonlyEntityGuidMappable<MovementInformation> MovementInformationMap { get; }

		/// <inheritdoc />
		public MovementUpdateMessageSender(
			[NotNull] IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> sessionMappable, 
			[NotNull] IReadonlyEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable, 
			[NotNull] IReadonlyEntityGuidMappable<MovementInformation> movementInformationMap)
		{
			SessionMappable = sessionMappable ?? throw new ArgumentNullException(nameof(sessionMappable));
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable ?? throw new ArgumentNullException(nameof(guidToInterestCollectionMappable));
			MovementInformationMap = movementInformationMap ?? throw new ArgumentNullException(nameof(movementInformationMap));
		}

		/// <inheritdoc />
		public void Send(EntityMovementMessageContext context)
		{
			//When they call this, they intend to send a movement update to the connection associated
			//with the NetworkEntityGuid provided so we lookup the session associated with it
			//as well as the interest list and movement data for each individual entry int he internest
			//collection to build the movement packet.
			if(!SessionMappable.ContainsKey(context.EntityGuid))
				throw new InvalidOperationException($"Tried to send movement update to Session with Guid: {context.EntityGuid} but none existed.");

			//it's possible they aren't interested or have an empty interest so they may have no collection.
			if(!GuidToInterestCollectionMappable.ContainsKey(context.EntityGuid))
				return;

			//TODO: Check if empty
			MovementDataUpdateEventPayload movementUpdateEvent = new MovementDataUpdateEventPayload(BuildMovementBlocks(context.EntityGuid));

			SessionMappable[context.EntityGuid].SendMessage(movementUpdateEvent);
		}

		//TODO: We need to filter in ONLY dirty movement data. Right now it resends movement data every packet but we only want to update if the data has changed.
		private AssociatedMovementInformation[] BuildMovementBlocks(NetworkEntityGuid guid)
		{
			return GuidToInterestCollectionMappable[guid]
				.ContainedEntities
				.Select(e => new AssociatedMovementInformation(e, MovementInformationMap[e]))
				.ToArray();
		}

		/// <inheritdoc />
		public Task SendAsync(EntityMovementMessageContext context)
		{
			throw new NotImplementedException("TODO: Implement the async version");
		}
	}
}
