﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	public interface IChatMessageBoxReciever
	{
		void ReceiveChatMessage(int tabId, string text);
	}
}
