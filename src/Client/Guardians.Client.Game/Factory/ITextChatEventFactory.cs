using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ITextChatEventFactory
	{
		TextChatEventData CreateChatData<TMessageType>(EntityAssociatedData<TMessageType> incomingChatMessageEventData)
			where TMessageType : ITextMessageContainable;

		TextChatEventData CreateChatData<TMessageType>(TMessageType incomingChatMessageEventData)
			where TMessageType : ITextMessageContainable;
	}
}
