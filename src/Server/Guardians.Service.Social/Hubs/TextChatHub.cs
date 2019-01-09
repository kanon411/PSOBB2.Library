using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	[AuthorizeJwt]
	public sealed class TextChatHub : AuthorizationReadySignalRHub
	{
		private ISocialServiceToGameServiceClient SocialToGameClient { get; }

		/// <inheritdoc />
		public TextChatHub(IClaimsPrincipalReader claimsReader, ILogger<AuthorizationReadySignalRHub> logger, [JetBrains.Annotations.NotNull] ISocialServiceToGameServiceClient socialToGameClient) 
			: base(claimsReader, logger)
		{
			SocialToGameClient = socialToGameClient ?? throw new ArgumentNullException(nameof(socialToGameClient));
		}

		public void Test(string message)
		{
			this.Clients.All.SendCoreAsync("Test", new object[1] { $"{this.Context.ConnectionId}: {message}"});
		}

		/// <inheritdoc />
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync()
				.ConfigureAwait(false);

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Account Connected: {ClaimsReader.GetUserName(Context.User)}:{ClaimsReader.GetUserId(Context.User)}");

			//We should never be here unless auth worked
			//so we can assume that and just try to request character session data
			//for the account.
			CharacterSessionDataResponse characterSessionDataResponse = await SocialToGameClient.GetCharacterSessionDataByAccount(ClaimsReader.GetUserIdInt(this.Context.User))
				.ConfigureAwait(false);

			//If the session data request fails we should just abort
			//and disconnect, the user shouldn't be connecting
			if(!characterSessionDataResponse.isSuccessful)
			{
				if(Logger.IsEnabled(LogLevel.Warning))
					Logger.LogWarning($"Failed to Query SessionData for AccountId: {ClaimsReader.GetUserId(Context.User)} Reason: {characterSessionDataResponse.ResultCode}");

				this.Context.Abort();
				return;
			}

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Recieved SessionData: Id: {characterSessionDataResponse.CharacterId} ZoneId: {characterSessionDataResponse.ZoneId}");
		}

		/// <inheritdoc />
		public override Task OnDisconnectedAsync(Exception exception)
		{
			return base.OnDisconnectedAsync(exception);
		}
	}
}
