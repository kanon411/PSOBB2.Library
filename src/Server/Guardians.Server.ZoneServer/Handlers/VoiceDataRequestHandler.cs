using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public sealed class VoiceDataRequestHandler : ControlledEntityRequestHandler<VoiceDataChangeRaiseRequestPayload>
	{
		private IReadonlyEntityGuidMappable<InterestCollection> InterestCollections { get; }

		private INetworkMessageSender<GenericSingleTargetMessageContext<VoiceDataChangeRaiseEventPayload>> VoiceMessageSender { get; }

		/// <inheritdoc />
		public VoiceDataRequestHandler(
			ILog logger, 
			IReadonlyConnectionEntityCollection connectionIdToEntityMap, 
			[NotNull] IReadonlyEntityGuidMappable<InterestCollection> interestCollections,
			[NotNull] INetworkMessageSender<GenericSingleTargetMessageContext<VoiceDataChangeRaiseEventPayload>> voiceMessageSender,
			IContextualResourceLockingPolicy<NetworkEntityGuid> lockingPolicy) 
			: base(logger, connectionIdToEntityMap, lockingPolicy)
		{
			InterestCollections = interestCollections ?? throw new ArgumentNullException(nameof(interestCollections));
			VoiceMessageSender = voiceMessageSender ?? throw new ArgumentNullException(nameof(voiceMessageSender));
		}

		//TODO: This is pretty slow for forwarding voice, however the plan is to make a GladNet3 (R)UDP service and work on low level API for efficiency.
		/// <inheritdoc />
		protected override async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, VoiceDataChangeRaiseRequestPayload payload, NetworkEntityGuid guid)
		{
			//TODO: This is not thread safe, ContainedEntities is a HashSet and this could break. We also need an efficient way to broadcast. Preferablly by pushing single serialized packet bytes to N clients.
			ProjectVersionStage.AssertAlpha();

			VoiceDataChangeRaiseEventPayload payloadToSend = new VoiceDataChangeRaiseEventPayload(new EntityAssociatedData<VoiceData>(guid, payload.Data));
			foreach(var interestedEntity in this.GetEntityMappedObject(InterestCollections, context).ContainedEntities)
			{
				await VoiceMessageSender.SendAsync(new GenericSingleTargetMessageContext<VoiceDataChangeRaiseEventPayload>(interestedEntity, payloadToSend))
					.ConfigureAwait(false);
			}
		}
	}
}
