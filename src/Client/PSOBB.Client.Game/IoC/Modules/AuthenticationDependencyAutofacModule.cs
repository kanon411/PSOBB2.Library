using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GladNet;
using Refit;
using UnityEngine;

namespace PSOBB
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
			builder.Register<IServiceDiscoveryService>(context => RestService.For<IServiceDiscoveryService>(ServiceDiscoveryUrl))
				.As<IServiceDiscoveryService>()
				.SingleInstance();

			builder.Register<IAuthenticationService>(context =>
			{
				IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

				return new AsyncEndpointAuthenticationService(QueryForRemoteServiceEndpoint(serviceDiscovery, "Authentication"), new RefitSettings() { HttpMessageHandlerFactory = () => new FiddlerEnabledWebProxyHandler() });
			});

			//TODO: We should do this only once, so we should move this to it's own special setup scene for one-time stuff.
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);
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
