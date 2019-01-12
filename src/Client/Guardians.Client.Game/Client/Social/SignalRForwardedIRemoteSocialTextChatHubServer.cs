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
				value.RegisterClientInterface<IRemoteSocialTextChatHubClient>(this);
				_connection = value;
			}
		}

		private INameQueryService NameQueryable { get; }

		private ITextChatEventFactory TextChatEventDataFactory { get; }

		//TODO: We don't want to directly expose a queue
		private Queue<TextChatEventData> ChatEventQueue { get; }

		/// <inheritdoc />
		public SignalRForwardedIRemoteSocialTextChatHubServer([NotNull] INameQueryService nameQueryable, [NotNull] ITextChatEventFactory textChatEventDataFactory, [NotNull] Queue<TextChatEventData> chatEventQueue)
		{
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
			TextChatEventDataFactory = textChatEventDataFactory ?? throw new ArgumentNullException(nameof(textChatEventDataFactory));
			ChatEventQueue = chatEventQueue ?? throw new ArgumentNullException(nameof(chatEventQueue));
		}

		public async Task RecieveZoneChannelTextChatMessageAsync(ZoneChatMessageEventModel message)
		{
			string entityName = await NameQueryable.RetrieveAsync(message.ChannelMessage.EntityGuid.EntityId)
				.ConfigureAwait(false);

			TextChatEventData chatData = TextChatEventDataFactory.CreateChatData(message.ChannelMessage, entityName);

			ChatEventQueue.Enqueue(chatData);
		}
	}
}