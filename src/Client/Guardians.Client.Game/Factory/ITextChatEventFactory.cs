using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ITextChatEventFactory
	{
		TextChatEventData CreateChatData<TMessageType>(EntityAssociatedData<TMessageType> incomingChatMessageEventData, string associatedEntityName)
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable;

		TextChatEventData CreateChatData<TMessageType>(TMessageType incomingChatMessageEventData)
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable;
	}
}
