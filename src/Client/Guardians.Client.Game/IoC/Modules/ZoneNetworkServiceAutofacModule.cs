using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using UnityEngine;

namespace Guardians
{
	public sealed class ZoneNetworkServiceAutofacModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.CheckCertificateRevocationList = false;

			/*builder.RegisterType<TypeSafeServiceDiscoveryServiceClient>()
				.As<IServiceDiscoveryService>()
				.WithParameter(new TypedParameter(typeof(string), @"http://localhost:5003"))
				.SingleInstance();

			builder.Register(context =>
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
			builder.Register(context => new CachedNameQueryServiceDecorator(new RemoteNetworkedNameQueryService(context.Resolve<ICharacterService>())))
				.As<INameQueryService>()
				.SingleInstance();*/

			//TODO: Unity 2018 issues with TypeSafe.Http.Net
			builder.RegisterInstance(new MockedINameQueryService())
				.As<INameQueryService>();

			builder.RegisterInstance(new MockedICharacterService())
				.As<ICharacterService>();
		}

		private class MockedICharacterService : ICharacterService
		{
			/// <inheritdoc />
			public Task<CharacterListResponse> GetCharacters(string authToken)
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc />
			public Task<NameQueryResponse> NameQuery(int characterId)
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc />
			public Task<CharacterSessionEnterResponse> TryEnterSession(int characterId, string authToken)
			{
				throw new NotImplementedException();
			}
		}

		private class MockedINameQueryService : INameQueryService
		{
			/// <inheritdoc />
			public string Retrieve(int id)
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc />
			public Task<string> RetrieveAsync(int id)
			{
				throw new NotImplementedException();
			}
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
