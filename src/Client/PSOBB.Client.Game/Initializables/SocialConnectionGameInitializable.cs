using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GladMMO
{

	[SceneTypeCreate(GameSceneType.ZoneGameScene)]
	public sealed class SocialConnectionGameInitializable : IGameInitializable
	{
		private IEnumerable<ISignalRConnectionHubInitializable> InitializableSocialServices { get; }

		private IServiceDiscoveryService ServiceDiscoveryService { get; }

		private ILocalPlayerDetails PlayerDetails { get; }

		private IReadonlyAuthTokenRepository AuthTokenProvider { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public SocialConnectionGameInitializable(
			[NotNull] IEnumerable<ISignalRConnectionHubInitializable> initializableSocialServices,
			[NotNull] IServiceDiscoveryService serviceDiscoveryService,
			[NotNull] ILocalPlayerDetails playerDetails,
			[NotNull] IReadonlyAuthTokenRepository authTokenProvider,
			[NotNull] ILog logger)
		{
			InitializableSocialServices = initializableSocialServices ?? throw new ArgumentNullException(nameof(initializableSocialServices));
			ServiceDiscoveryService = serviceDiscoveryService ?? throw new ArgumentNullException(nameof(serviceDiscoveryService));
			PlayerDetails = playerDetails ?? throw new ArgumentNullException(nameof(playerDetails));
			AuthTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			//We need to connect the hub to the social backend
			ResolveServiceEndpointResponse endpointResponse = await ServiceDiscoveryService.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, "SocialService"))
				.ConfigureAwait(false);

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for SocialService. Reason: {endpointResponse.ResultCode}");

			string hubConnectionString = $@"{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/realtime/textchat";

			if(Logger.IsInfoEnabled)
				Logger.Info($"Social HubConnection String: {hubConnectionString}");

			//TODO: Handle failed service disc query
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl(hubConnectionString, options =>
					{
						options.Headers.Add(SocialNetworkConstants.CharacterIdRequestHeaderName, PlayerDetails.LocalPlayerGuid.EntityId.ToString());
						options.AccessTokenProvider = () => Task.FromResult(AuthTokenProvider.Retrieve());
					})
				.AddJsonProtocol()
				.Build();

			foreach(var i in InitializableSocialServices)
			{
				i.Connection = connection;
			}

			//Just start the service when the game initializes
			//This will make it so that the signalR clients will start to recieve messages.
			await connection.StartAsync()
				.ConfigureAwait(false);
		}
	}
}
