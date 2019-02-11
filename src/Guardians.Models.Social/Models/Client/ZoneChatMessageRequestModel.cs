using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	/// <summary>
	/// Data model for <see cref="TargetlessChannelChatMessageRequestModel"/>
	/// for the <see cref="ChatChannels"/>.ZoneChannel.
	/// </summary>
	[JsonObject]
	public sealed class ZoneChatMessageRequestModel : TargetlessChannelChatMessageRequestModel
	{
		/// <inheritdoc />
		public ZoneChatMessageRequestModel(string message) 
			: base(message, ChatChannels.ZoneChannel)
		{

		}

		private ZoneChatMessageRequestModel()
			: base()
		{
			
		}
	}
}
