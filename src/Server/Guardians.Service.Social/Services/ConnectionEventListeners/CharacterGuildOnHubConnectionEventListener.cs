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

		//TODO: We don't want to directly expose the response
		private IEntityGuidMappable<CharacterGuildMembershipStatusResponse> GuildStatusMappable { get; }

		/// <inheritdoc />
		public CharacterGuildOnHubConnectionEventListener([JetBrains.Annotations.NotNull] ILogger<CharacterGuildOnHubConnectionEventListener> logger, [JetBrains.Annotations.NotNull] ISocialServiceToGameServiceClient socialToGameClient, [JetBrains.Annotations.NotNull] IClaimsPrincipalReader claimsReader, IEntityGuidMappable<CharacterGuildMembershipStatusResponse> guildStatusMappable)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			SocialToGameClient = socialToGameClient ?? throw new ArgumentNullException(nameof(socialToGameClient));
			ClaimsReader = claimsReader ?? throw new ArgumentNullException(nameof(claimsReader));
			GuildStatusMappable = guildStatusMappable;
		}

		/// <inheritdoc />
		public async Task<HubOnConnectionState> OnConnected(Hub hubConnectedTo)
		{
			//TODO: Verify that the character they requested is owned by them.
			ProjectVersionStage.AssertAlpha();

			CharacterGuildMembershipStatusResponse response = null;

			try
			{
				response = await SocialToGameClient.GetCharacterMembershipGuildStatus(int.Parse(hubConnectedTo.Context.UserIdentifier))
					.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				if(Logger.IsEnabled(LogLevel.Error))
					Logger.LogError($"Failed to get guild status of Connection: {hubConnectedTo.Context.UserIdentifier}. Exception: {e.Message}\n\nStack:{e.StackTrace}");

				return HubOnConnectionState.Abort;
			}

			NetworkEntityGuid guid = new NetworkEntityGuidBuilder()
				.WithId(int.Parse(hubConnectedTo.Context.UserIdentifier))
				.WithType(EntityType.Player)
				.Build();

			//TODO: We don't really want to directly expose this
			GuildStatusMappable.Add(guid, response);

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
