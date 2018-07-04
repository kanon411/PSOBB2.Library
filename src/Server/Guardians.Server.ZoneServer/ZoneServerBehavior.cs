using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace Guardians
{
	public sealed class ZoneServerBehavior : MonoBehaviour
	{
		private async Task Start()
		{
			ZoneServerApplicationBase appBase = new ZoneServerApplicationBase(new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 5006), new UnityLogger(LogLevel.Debug));

			if(!appBase.StartServer())
				throw new InvalidOperationException($"Failed to start server on Details: {appBase.ServerAddress}");

			await appBase.BeginListening()
				.ConfigureAwait(false);

			if(appBase.Logger.IsWarnEnabled)
				appBase.Logger.Warn($"Server is shutting down.");
		}
	}
}
