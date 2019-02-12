using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface INetworkConnectionEstablishedEventSubscribable
	{
		event EventHandler OnNetworkConnectionEstablished;
	}
}
