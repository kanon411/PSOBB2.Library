using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GladNet;

namespace Guardians
{
	public sealed class ZoneMessageBroadcastMessageHandler : IPeerPayloadSpecificMessageHandler<ZoneChatMessageRequestModel, object, HubConnectionMessageContext<IRemoteSocialTextChatHubClient>>
	{
		//I am not happy about this, but we need to maintain some state so that we know what zone a connection is in.
		private IConnectionToZoneMappable ZoneLookupService { get; }

		/// <inheritdoc />
		public ZoneMessageBroadcastMessageHandler([JetBrains.Annotations.NotNull] IConnectionToZoneMappable zoneLookupService)
		{
			ZoneLookupService = zoneLookupService ?? throw new ArgumentNullException(nameof(zoneLookupService));
		}

		/// <inheritdoc />
		public async Task HandleMessage(HubConnectionMessageContext<IRemoteSocialTextChatHubClient> context, ZoneChatMessageRequestModel message)
		{
			//TODO: We may want to do validation for the message sent more than this
			if(message.TargetChannel != ChatChannels.ZoneChannel)
				return;

			if(String.IsNullOrWhiteSpace(message.Message))
				return;

			//Send only to same zone
			//TODO: Have a group name builder, don't hardcore
			await GetCallerZoneGroup(context).RecieveZoneChannelTextChatMessageAsync(new ZoneChatMessageEventModel(BuildForwardableTargetlessChannelChatMessage(context, message)));
		}

		private IRemoteSocialTextChatHubClient GetCallerZoneGroup(HubConnectionMessageContext<IRemoteSocialTextChatHubClient> context)
		{
			return context.Clients.Group($"zone:{ZoneLookupService.Retrieve(context.HubConntext.ConnectionId)}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private EntityAssociatedData<TargetlessChannelChatMessageRequestModel> BuildForwardableTargetlessChannelChatMessage(HubConnectionMessageContext<IRemoteSocialTextChatHubClient> context, TargetlessChannelChatMessageRequestModel message) => BuildForwardableAssociatedData(context, message);

		private EntityAssociatedData<T> BuildForwardableAssociatedData<T>([JetBrains.Annotations.NotNull] HubConnectionMessageContext<IRemoteSocialTextChatHubClient> context, [JetBrains.Annotations.NotNull] T envolpeContents)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));
			if(envolpeContents == null) throw new ArgumentNullException(nameof(envolpeContents));

			//TODO: We should cache somehow the identifier's int value, parsing it each time I think can be costly.
			NetworkEntityGuid guid = new NetworkEntityGuidBuilder()
				.WithId(int.Parse(context.HubConntext.UserIdentifier))
				.WithType(EntityType.Player)
				.Build();

			return new EntityAssociatedData<T>(guid, envolpeContents);
		}
	}
}
