using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public interface IRemoteSocialTextChatHubClient
	{
		/// <summary>
		/// Async method that handles recieveing a <see cref="TargetlessChannelChatMessageEventModel"/>
		/// (a zone text chat message) with information about the sender.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <returns>Awaitable.</returns>
		Task RecieveZoneChannelTextChatMessageAsync(TargetlessChannelChatMessageEventModel message);
	}
}
