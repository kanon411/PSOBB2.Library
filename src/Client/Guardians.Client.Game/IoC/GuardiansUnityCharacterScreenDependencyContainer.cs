using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
//using PostSharp.Patterns.Caching;
//using PostSharp.Patterns.Caching.Backends;
using SceneJect.Common;
using TypeSafe.Http.Net;
using UnityEngine;

namespace Guardians
{
	//TODO: Extract this into an Autofac module
	public sealed class GuardiansUnityCharacterScreenDependencyContainer : NonBehaviourDependency
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			//Set the sync context
			UnityExtended.InitializeSyncContext();

			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.CheckCertificateRevocationList = false;

			//Postsharp requires we setup some backend stuff
			//CachingServices.DefaultBackend = new MemoryCachingBackend();

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

			register.RegisterType<LocalCharacterDataRepository>()
				.As<ICharacterDataRepository>()
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

		//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
		private bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}
	}
}
