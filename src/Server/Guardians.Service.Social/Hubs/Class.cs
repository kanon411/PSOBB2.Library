using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Guardians
{
	public sealed class TestHub : Hub
	{
		public void Test(string message)
		{
			this.Clients.All.SendCoreAsync("Test", new object[1] { $"{this.Context.ConnectionId}: {message}"});
		}
	}
}
