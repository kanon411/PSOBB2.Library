using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GladNet;
using TypeSafe.Http.Net;
using UnityEngine;

namespace Guardians
{
	public sealed class AuthenticationDependencyAutofacModule : Module
	{
		/// <summary>
		/// The URl for service discovery. This is important
		/// because the authentication service needs to know where its endpoint is
		/// and can't without querying the service discovery service first.
		/// </summary>
		private string ServiceDiscoveryUrl { get; }

		public AuthenticationDependencyAutofacModule()
		{
			//Default registeration will result in a MOCK URL
			ServiceDiscoveryUrl = @"http://localhost:80";
		}

		/// <inheritdoc />
		public AuthenticationDependencyAutofacModule(string serviceDiscoveryUrl)
		{
			if(string.IsNullOrWhiteSpace(serviceDiscoveryUrl)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceDiscoveryUrl));

			ServiceDiscoveryUrl = serviceDiscoveryUrl;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<TypeSafeServiceDiscoveryServiceClient>()
				.As<IServiceDiscoveryService>()
				.WithParameter(new TypedParameter(typeof(string), ServiceDiscoveryUrl))
				.SingleInstance();

			builder.Register<IAuthenticationService>(context =>
			{
				IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

				return TypeSafeHttpBuilder<IAuthenticationService>
					.Create()
					.RegisterDefaultSerializers()
					.RegisterJsonNetSerializer()
					.RegisterDotNetHttpClient(QueryForRemoteServiceEndpoint(serviceDiscovery, "Authentication"), new FiddlerEnabledWebProxyHandler())
					.Build();
			});
		}

		//TODO: Put this in a base class or something
		private async Task<string> QueryForRemoteServiceEndpoint(IServiceDiscoveryService serviceDiscovery, string serviceType)
		{
			ResolveServiceEndpointResponse endpointResponse = await serviceDiscovery.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceType));

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for Service: {serviceType} Result: {endpointResponse.ResultCode}");

			//TODO: Logging
			//Debug.Log($"Recieved service discovery response: {endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort} for Type: {serviceType}");

			//TODO: Do we need extra slash?
			return $"{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/";
		}
	}
}
