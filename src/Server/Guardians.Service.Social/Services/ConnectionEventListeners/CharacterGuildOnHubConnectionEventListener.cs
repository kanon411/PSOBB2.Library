using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	public sealed class CharacterGuildOnHubConnectionEventListener : IOnHubConnectionEventListener
	{
		private ILogger<CharacterGuildOnHubConnectionEventListener> Logger { get; }

		private ISocialServiceToGameServiceClient SocialToGameClient { get; }

		private IClaimsPrincipalReader ClaimsReader { get; }

		/// <inheritdoc />
		public CharacterGuildOnHubConnectionEventListener([JetBrains.Annotations.NotNull] ILogger<CharacterGuildOnHubConnectionEventListener> logger, [JetBrains.Annotations.NotNull] ISocialServiceToGameServiceClient socialToGameClient, [JetBrains.Annotations.NotNull] IClaimsPrincipalReader claimsReader)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			SocialToGameClient = socialToGameClient ?? throw new ArgumentNullException(nameof(socialToGameClient));
			ClaimsReader = claimsReader ?? throw new ArgumentNullException(nameof(claimsReader));
		}

		/// <inheritdoc />
		public async Task<HubOnConnectionState> OnConnected(Hub hubConnectedTo)
		{
			//TODO: Verify that the character they requested is owned by them.
			ProjectVersionStage.AssertAlpha();

			CharacterGuildMembershipStatusResponse response = null;

			try
			{
				response = await SocialToGameClient.GetCharacterMembershipGuildStatus(ClaimsReader.GetUserIdInt(hubConnectedTo.Context.User))
					.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				//TODO: Log.
				return HubOnConnectionState.Abort;
			}

			if(response.isSuccessful)
			{
				//TODO: don't hardcode
				await hubConnectedTo.Groups.AddToGroupAsync(hubConnectedTo.Context.ConnectionId, $"guild:{response.GuildId}")
					.ConfigureAwait(false);
			}
			else
				return HubOnConnectionState.Error;

			return HubOnConnectionState.Success;
		}
	}
}
