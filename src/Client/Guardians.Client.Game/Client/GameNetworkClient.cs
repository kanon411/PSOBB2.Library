using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using Guardians;
using SceneJect.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// The component that manages the game network client.
	/// </summary>
	[Injectee]
	public sealed class GameNetworkClient : BaseUnityNetworkClient<GameServerPacketPayload, GameClientPacketPayload>
	{
		//TODO: Handle connection details better than this
		/// <summary>
		/// Data model for connection details.
		/// </summary>
		//[Inject]
		//private IGameConnectionEndpointDetails ConnectionEndpoint { get; }

		[Tooltip("Indicates if the client should connect on Start.")]
		[SerializeField]
		private bool ConnectOnStart = false;

		private void Start()
		{
			if(ConnectOnStart)
				StartConnection();
		}

		//TODO: Is it safe or ok to await in start without ever completing?
		public void StartConnection()
		{
			//Just start the startup task.
			Task.Factory.StartNew(StartNetworkClient, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
				.ConfigureAwait(true);
		}

		//Starts the client by connecting
		//If connection seems to succeed it will continue and startup the full client
		private async Task StartNetworkClient()
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug("Starting game client");

			//TODO: Don't hardcode gameserver connection details
			//As soon as we start we should attempt to connect to the login server.
			bool result = await Client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 5006)
				.ConfigureAwait(true);

			if(!result)
				throw new InvalidOperationException($"Failed to connect to Server: {"127.0.0.1"} Port: {5006}");

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Connected client. isConnected: {Client.isConnected}");

			CreateDispatchTask();
		}
	}
}
