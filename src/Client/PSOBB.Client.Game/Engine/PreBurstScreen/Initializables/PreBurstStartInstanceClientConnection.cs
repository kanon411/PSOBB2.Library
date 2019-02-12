using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.PreZoneBurstingScreen)]
	public sealed class PreBurstStartInstanceClientConnection : IGameInitializable
	{
		/// <summary>
		/// The managed network client that the Unity3D client is implemented on-top of.
		/// </summary>
		private IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> Client { get; }

		private ICharacterService CharacterDataService { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepo { get; }

		private ICharacterDataRepository CharacterDataRepo { get; }

		private IZoneServerService ZoneService { get; }

		private ILog Logger { get; }

		private INetworkClientManager NetworkClientManager { get; }

		/// <inheritdoc />
		public PreBurstStartInstanceClientConnection([NotNull] IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> client, [NotNull] ICharacterService characterDataService, [NotNull] IReadonlyAuthTokenRepository authTokenRepo, [NotNull] ICharacterDataRepository characterDataRepo, [NotNull] IZoneServerService zoneService, [NotNull] ILog logger)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			CharacterDataService = characterDataService ?? throw new ArgumentNullException(nameof(characterDataService));
			AuthTokenRepo = authTokenRepo ?? throw new ArgumentNullException(nameof(authTokenRepo));
			CharacterDataRepo = characterDataRepo ?? throw new ArgumentNullException(nameof(characterDataRepo));
			ZoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		//TODO: We need to handle failure cases, maybe with a window popup and bringing back to the titlescreen.
		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			//When we reach this scene, the pre lobby burst scene
			//we need to actually connect to the zone/lobby.
			//it verry well could be a zone. Maybe we were in a party and are reconnecting to it
			//no matter what though, we need to get information about our
			//character session and then the zone it should be connecting to
			//then we can connect.

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

			//Basically we just take the network client and tell the client manager to start dealing with it
			//since it's connecting the manager should start pumping the messages out of it.
			await NetworkClientManager.StartHandlingNetworkClient(Client)
				.ConfigureAwait(true);
		}
	}
}
