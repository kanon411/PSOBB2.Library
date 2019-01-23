using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[AuthorizeJwt]
	public sealed class TextChatHub : AuthorizationReadySignalRHub<IRemoteSocialTextChatHubClient>, IRemoteSocialTextChatHubServer
	{
		private ISocialServiceToGameServiceClient SocialToGameClient { get; }

		//I am not happy about this, but we need to maintain some state so that we know what zone a connection is in.
		private IConnectionToZoneMappable ZoneLookupService { get; }

		private IEnumerable<IOnHubConnectionEventListener> OnConnectionHubListeners { get; }

		/// <inheritdoc />
		public TextChatHub(IClaimsPrincipalReader claimsReader, 
			ILogger<TextChatHub> logger, 
			[JetBrains.Annotations.NotNull] ISocialServiceToGameServiceClient socialToGameClient,
			[JetBrains.Annotations.NotNull] IConnectionToZoneMappable zoneLookupService,
			[JetBrains.Annotations.NotNull] IEnumerable<IOnHubConnectionEventListener> onConnectionHubListeners) 
			: base(claimsReader, logger)
		{
			SocialToGameClient = socialToGameClient ?? throw new ArgumentNullException(nameof(socialToGameClient));
			ZoneLookupService = zoneLookupService ?? throw new ArgumentNullException(nameof(zoneLookupService));
			OnConnectionHubListeners = onConnectionHubListeners ?? throw new ArgumentNullException(nameof(onConnectionHubListeners));
		}

		/// <inheritdoc />
		public async Task SendZoneChannelTextChatMessageAsync(ZoneChatMessageRequestModel message)
		{
			//TODO: We may want to do validation for the message sent more than this
			if(message.TargetChannel != ChatChannels.ZoneChannel)
				return;

			if(String.IsNullOrWhiteSpace(message.Message))
				return;

			//Send only to same zone
			//TODO: Have a group name builder, don't hardcore
			await GetCallerZoneGroup().RecieveZoneChannelTextChatMessageAsync(new ZoneChatMessageEventModel(BuildForwardableTargetlessChannelChatMessage(message)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private EntityAssociatedData<TargetlessChannelChatMessageRequestModel> BuildForwardableTargetlessChannelChatMessage(TargetlessChannelChatMessageRequestModel message) => BuildForwardableAssociatedData(message);

		private EntityAssociatedData<T> BuildForwardableAssociatedData<T>([JetBrains.Annotations.NotNull] T envolpeContents)
		{
			if(envolpeContents == null) throw new ArgumentNullException(nameof(envolpeContents));

			//TODO: We should cache somehow the identifier's int value, parsing it each time I think can be costly.
			NetworkEntityGuid guid = new NetworkEntityGuidBuilder()
				.WithId(int.Parse(Context.UserIdentifier))
				.WithType(EntityType.Player)
				.Build();

			return new EntityAssociatedData<T>(guid, envolpeContents);
		}

		private IRemoteSocialTextChatHubClient GetCallerZoneGroup()
		{
			return this.Clients.Group($"zone:{ZoneLookupService.Retrieve(Context.ConnectionId)}");
		}

		/// <inheritdoc />
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync()
				.ConfigureAwait(false);

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Account Connected: {ClaimsReader.GetUserName(Context.User)}:{ClaimsReader.GetUserId(Context.User)}");

			try
			{
				foreach(var listener in OnConnectionHubListeners)
				{
					HubOnConnectionState connectionState = await listener.OnConnected(this).ConfigureAwait(false);

					//if the listener indicated we need to abort for whatever reason we
					//should believe it and just abort.
					if(connectionState == HubOnConnectionState.Abort)
					{
						Context.Abort();
						break;
					}
				}
			}
			catch(Exception e)
			{
				if(Logger.IsEnabled(LogLevel.Error))
					Logger.LogInformation($"Account: {ClaimsReader.GetUserName(Context.User)}:{ClaimsReader.GetUserId(Context.User)} failed to properly connect to hub. Error: {e.Message}\n\nStack: {e.StackTrace}");

				Context.Abort();
			}
		}

		/// <inheritdoc />
		public override Task OnDisconnectedAsync(Exception exception)
		{
			if(ZoneLookupService.Contains(Context.ConnectionId))
				ZoneLookupService.Unregister(Context.ConnectionId);

			return base.OnDisconnectedAsync(exception);
		}
	}
}
