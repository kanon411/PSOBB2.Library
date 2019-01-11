using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Guardians
{

	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene)]
	public sealed class SocialConnectionGameInitializable : IGameInitializable
	{
		private IEnumerable<ISignalRConnectionHubInitializable> InitializableSocialServices { get; }

		private IServiceDiscoveryService ServiceDiscoveryService { get; }

		/// <inheritdoc />
		public SocialConnectionGameInitializable(
			[NotNull] IEnumerable<ISignalRConnectionHubInitializable> initializableSocialServices,
			[NotNull] IServiceDiscoveryService serviceDiscoveryService)
		{
			InitializableSocialServices = initializableSocialServices ?? throw new ArgumentNullException(nameof(initializableSocialServices));
			ServiceDiscoveryService = serviceDiscoveryService ?? throw new ArgumentNullException(nameof(serviceDiscoveryService));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			//We need to connect the hub to the social backend
			ResolveServiceEndpointResponse endpointResponse = await ServiceDiscoveryService.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, "SocialService"))
				.ConfigureAwait(false);

			//TODO: Handle failed service disc query
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl($"http://{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/realtime/textchat")
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
