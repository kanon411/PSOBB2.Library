using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class VisibilityChangeMessageSender : INetworkMessageSender<EntityVisibilityChangeContext>
	{
		private IReadonlyEntityGuidMappable<MovementInformation> MovementInformationMappable { get; }

		private IReadonlyEntityGuidMappable<ZoneClientSession> SessionMappable { get; }

		/// <inheritdoc />
		public VisibilityChangeMessageSender([NotNull] IReadonlyEntityGuidMappable<MovementInformation> movementInformationMappable, [NotNull] IReadonlyEntityGuidMappable<ZoneClientSession> sessionMappable)
		{
			MovementInformationMappable = movementInformationMappable ?? throw new ArgumentNullException(nameof(movementInformationMappable));
			SessionMappable = sessionMappable ?? throw new ArgumentNullException(nameof(sessionMappable));
		}

		/// <inheritdoc />
		public void Send([NotNull] EntityVisibilityChangeContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			NetworkObjectVisibilityChangeEventPayload changeEventPayload = BuildPayload(context.InterestCollection);
			ZoneClientSession session = RetrieveSession(context.EntityGuid);

			session.SendService.SendMessage(changeEventPayload, DeliveryMethod.ReliableOrdered)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task SendAsync([NotNull] EntityVisibilityChangeContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			NetworkObjectVisibilityChangeEventPayload changeEventPayload = BuildPayload(context.InterestCollection);
			ZoneClientSession session = RetrieveSession(context.EntityGuid);

			//TODO: Should we await or return?
			await session.SendService.SendMessage(changeEventPayload, DeliveryMethod.ReliableOrdered)
				.ConfigureAwait(false);
		}

		private ZoneClientSession RetrieveSession([NotNull] NetworkEntityGuid guid)
		{
			if(guid == null) throw new ArgumentNullException(nameof(guid));

			//TODO: This will happen under disconnection circumstances. We need better disconnection handling.
			if(!SessionMappable.ContainsKey(guid))
				throw new InvalidOperationException($"Session that owns: {guid} no longer exists.");

			ZoneClientSession session = SessionMappable[guid];
			return session;
		}

		private NetworkObjectVisibilityChangeEventPayload BuildPayload([NotNull] IReadonlyInterestCollection interestCollection)
		{
			if(interestCollection == null) throw new ArgumentNullException(nameof(interestCollection));

			//TODO: Provide movement mappable
			InterestChangedPacketBuilder changedPacketBuilder = new InterestChangedPacketBuilder(MovementInformationMappable);

			//We delegate the packet building to the packet builder. But we still need to send it.
			//Sending is async so it can be fired off and not awaited, we won't want to await it
			NetworkObjectVisibilityChangeEventPayload changeEventPayload = changedPacketBuilder.Build(interestCollection.QueuedJoiningEntities, interestCollection.QueuedLeavingEntities);
			return changeEventPayload;
		}
	}
}
