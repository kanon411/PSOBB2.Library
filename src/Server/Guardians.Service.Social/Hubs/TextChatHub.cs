using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GladNet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[AuthorizeJwt]
	public sealed class TextChatHub : AuthorizationReadySignalRHub<IRemoteSocialTextChatHubClient>, 
		IRemoteSocialTextChatHubServer, 
		IConnectionService, 
		IPeerPayloadSendService<object>, 
		IPeerRequestSendService<object>
	{
		private ISocialServiceToGameServiceClient SocialToGameClient { get; }

		//I am not happy about this, but we need to maintain some state so that we know what zone a connection is in.
		private IConnectionToZoneMappable ZoneLookupService { get; }

		private IEnumerable<IOnHubConnectionEventListener> OnConnectionHubListeners { get; }

		/// <summary>
		/// This is the dependency injection service creator.
		/// It should ONLY be used to create handlers for messages.
		/// </summary>
		private IServiceProvider ServiceProvider { get; }

		/// <inheritdoc />
		public TextChatHub(IClaimsPrincipalReader claimsReader, 
			ILogger<TextChatHub> logger, 
			[JetBrains.Annotations.NotNull] ISocialServiceToGameServiceClient socialToGameClient,
			[JetBrains.Annotations.NotNull] IConnectionToZoneMappable zoneLookupService,
			[JetBrains.Annotations.NotNull] IEnumerable<IOnHubConnectionEventListener> onConnectionHubListeners,
			[JetBrains.Annotations.NotNull] IServiceProvider serviceProvider) 
			: base(claimsReader, logger)
		{
			SocialToGameClient = socialToGameClient ?? throw new ArgumentNullException(nameof(socialToGameClient));
			ZoneLookupService = zoneLookupService ?? throw new ArgumentNullException(nameof(zoneLookupService));
			OnConnectionHubListeners = onConnectionHubListeners ?? throw new ArgumentNullException(nameof(onConnectionHubListeners));
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		/// <inheritdoc />
		public async Task SendZoneChannelTextChatMessageAsync(ZoneChatMessageRequestModel message)
		{
			ZoneMessageBroadcastMessageHandler handler = ServiceProvider.GetService<ZoneMessageBroadcastMessageHandler>();

			await handler.HandleMessage(CreateHubContext(), message)
				.ConfigureAwait(false);
		}

		private HubConnectionMessageContext<IRemoteSocialTextChatHubClient> CreateHubContext()
		{
			return new HubConnectionMessageContext<IRemoteSocialTextChatHubClient>(this, this, this, Groups, Clients, Context);
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

		/// <inheritdoc />
		public Task DisconnectAsync(int delay)
		{
			Context.Abort();
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public Task<bool> ConnectAsync(string ip, int port)
		{
			throw new NotSupportedException($"This does not make sense for SignalR.");
		}

		/// <inheritdoc />
		public bool isConnected => !Context.ConnectionAborted.IsCancellationRequested;

		/// <inheritdoc />
		Task<SendResult> IPeerPayloadSendService<object>.SendMessage<TPayloadType>(TPayloadType payload, DeliveryMethod method)
		{
			throw new NotSupportedException($"This does not make sense for SignalR.");
		}

		/// <inheritdoc />
		Task<SendResult> IPeerPayloadSendService<object>.SendMessageImmediately<TPayloadType>(TPayloadType payload, DeliveryMethod method)
		{
			throw new NotSupportedException($"This does not make sense for SignalR.");
		}

		/// <inheritdoc />
		Task<TResponseType> IPeerRequestSendService<object>.SendRequestAsync<TResponseType>(object request, DeliveryMethod method, CancellationToken cancellationToken)
		{
			throw new NotSupportedException($"This does not make sense for SignalR.");
		}
	}
}
