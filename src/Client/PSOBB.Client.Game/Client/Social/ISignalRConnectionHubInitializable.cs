using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace PSOBB
{
	public interface ISignalRConnectionHubInitializable
	{
		HubConnection Connection { get; set; }
	}
}
