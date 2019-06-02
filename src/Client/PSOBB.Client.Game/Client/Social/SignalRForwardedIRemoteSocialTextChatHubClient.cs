using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace PSOBB
{
	public sealed class SignalRForwardedIRemoteSocialTextChatHubClient : IRemoteSocialTextChatHubServer, ISignalRConnectionHubInitializable
	{
		//Externally initializable
		[UsedImplicitly]
		public HubConnection Connection { get; set; }

		/// <inheritdoc />
		public Task SendZoneChannelTextChatMessageAsync(ZoneChatMessageRequestModel message)
		{
			return Connection.SendAsync(nameof(SendZoneChannelTextChatMessageAsync), message);
		}

		/// <inheritdoc />
		public Task SendGuildChannelTextChatMessageAsync(GuildChatMessageRequestModel message)
		{
			return Connection.SendAsync(nameof(SendGuildChannelTextChatMessageAsync), message);
		}
	}
}
