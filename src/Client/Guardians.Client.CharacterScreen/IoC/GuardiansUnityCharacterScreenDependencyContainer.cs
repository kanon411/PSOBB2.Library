using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using SceneJect.Common;
using TypeSafe.Http.Net;
using UnityEngine;

namespace Guardians
{
	public sealed class GuardiansUnityCharacterScreenDependencyContainer : NonBehaviourDependency
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			ServiceCollection services = new ServiceCollection();

			register.RegisterInstance(new UnityLogger(LogLevel.All))
				.As<ILog>()
				.SingleInstance();

			register.RegisterType<AuthenticationTokenRepository>()
				.As<IReadonlyAuthTokenRepository>()
				.As<IAuthTokenRepository>()
				.SingleInstance();

			register.RegisterType<TypeSafeServiceDiscoveryServiceClient>()
				.As<IServiceDiscoveryService>()
				.WithParameter(new TypedParameter(typeof(string), ServiceDiscoveryUrl))
				.SingleInstance();

			register.Register(context =>
				{
					//The below is not true for right now, we have global service discovery point to the gameserver for testing.
					//This registeration is abit complicated
					//because we are skipping the game server selection
					//to do this we must query the service discovery and THEN
					//we query the the gameserver's service discovery.
					IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

					return new RemoteNetworkCharacterService(QueryForRemoteServiceEndpoint(serviceDiscovery, "GameServer"));
				})
				.As<ICharacterService>()
				.SingleInstance();

			//Name query service
			register.Register(context => new CachedNameQueryServiceDecorator(new RemoteNetworkedNameQueryService(context.Resolve<ICharacterService>())))
				.As<INameQueryService>()
				.SingleInstance();

			register.Populate(services);
		}

		//TODO: Put this in a base class or something
		private async Task<string> QueryForRemoteServiceEndpoint(IServiceDiscoveryService serviceDiscovery, string serviceType)
		{
			ResolveServiceEndpointResponse endpointResponse = await serviceDiscovery.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceType));

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for Service: {serviceType} Result: {endpointResponse.ResultCode}");

			Debug.Log($"Recieved service discovery response: {endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort} for Type: {serviceType}");

			//TODO: Do we need extra slash?
			return $"{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/";
		}
	}
}
