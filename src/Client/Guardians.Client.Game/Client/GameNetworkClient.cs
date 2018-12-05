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

		[Inject]
		private ICharacterService CharacterDataService { get; set; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; set; }

		[Inject]
		private ICharacterDataRepository CharacterDataRepo { get; set; }

		[Inject]
		private IZoneServerService ZoneService { get; set; }

		[Tooltip("Indicates if the client should connect on Start.")]
		[SerializeField]
		private bool ConnectOnStart = false;

		private async Task Start()
		{
			if(ConnectOnStart)
				await StartNetworkClient()
					.ConfigureAwait(true); //we want the network client to dispatch on the current thread. At least I think, been awhile since I saw GladNet source.
		}

		//TODO: Is it safe or ok to await in start without ever completing?
		public async Task StartConnection()
		{
			//Just start the startup task.
			
		}

		//Starts the client by connecting
		//If connection seems to succeed it will continue and startup the full client
		private async Task StartNetworkClient()
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug("Starting game client");

			//First we need to know what zone this session should be going to
			CharacterSessionDataResponse sessionData = await CharacterDataService.GetCharacterSessionData(CharacterDataRepo.CharacterId, AuthTokenRepo.RetrieveWithType())
				.ConfigureAwait(true);

			//TODO: Handle this better
			if(!sessionData.isSuccessful)
			{
				Logger.Error($"Failed to query session data for Character: {CharacterDataRepo.CharacterId}. Cannot connect to instance server.");
				return;
			}

			ResolveServiceEndpointResponse zoneEndpointResponse = await ZoneService.GetServerEndpoint(sessionData.ZoneId);

			if(!zoneEndpointResponse.isSuccessful)
			{
				Logger.Error($"Failed to query endpoint for Zone: {sessionData.ZoneId} which Character: {CharacterDataRepo.CharacterId} is in. Cannot connect to instance server.");
				return;
			}

			//TODO: Don't hardcode gameserver connection details
			//As soon as we start we should attempt to connect to the login server.
			bool result = await Client.ConnectAsync(IPAddress.Parse(zoneEndpointResponse.Endpoint.EndpointAddress), zoneEndpointResponse.Endpoint.EndpointPort)
				.ConfigureAwait(true);

			if(!result)
				throw new InvalidOperationException($"Failed to connect to Server: {zoneEndpointResponse.Endpoint.EndpointAddress} Port: {zoneEndpointResponse.Endpoint.EndpointPort}");

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Connected client. isConnected: {Client.isConnected}");

			CreateDispatchTask();
		}
	}
}
