using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Guardians
{
	public sealed class SignalRForwardedIRemoteSocialTextChatHubServer : IRemoteSocialTextChatHubClient, ISignalRConnectionHubInitializable
	{
		private HubConnection _connection;

		/// <inheritdoc />
		public HubConnection Connection
		{
			get => _connection;
			set
			{
				//TODO: What is this is being nulled out??
				//This part is pretty important, we want this to be the reciever of the callbacks.
				value.RegisterClientInterface(this);
				_connection = value;
			}
		}

		private INameQueryService NameQueryable { get; }

		/// <inheritdoc />
		public SignalRForwardedIRemoteSocialTextChatHubServer([NotNull] INameQueryService nameQueryable)
		{
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
		}

		public async Task RecieveZoneChannelTextChatMessageAsync(ZoneChatMessageEventModel message)
		{
			string entityName = await NameQueryable.RetrieveAsync(message.ChannelMessage.EntityGuid.EntityId)
				.ConfigureAwait(false);

			//TODO: Would performance be better if the server did this?
			string renderableMessage = $"[1. Zone] {entityName}: {message.ChannelMessage.Data.Message}";
		}
	}
}