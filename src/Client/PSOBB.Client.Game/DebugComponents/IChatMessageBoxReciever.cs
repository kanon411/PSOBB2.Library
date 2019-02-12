using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IChatMessageBoxReciever
	{
		void ReceiveChatMessage(int tabId, string text);
	}
}
