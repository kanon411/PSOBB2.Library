using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Logging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SceneJect.Common;
using TypeSafe.Http.Net;
using UnityEngine;

namespace Guardians
{
	public sealed class GuardiansUnityAuthDependencyContainer : NonBehaviourDependency
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			ServiceCollection services = new ServiceCollection();

			register.RegisterType<GuardiansUnityAuthenticationClient>()
				.As<IAuthenticationClient>()
				.SingleInstance();

			register.RegisterType<TypeSafeServiceDiscoveryServiceClient>()
				.As<IServiceDiscoveryService>()
				.WithParameter(new TypedParameter(typeof(string), ServiceDiscoveryUrl))
				.SingleInstance();

			register.Register<IAuthenticationService>(context =>
			{
				IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

				return TypeSafeHttpBuilder<IAuthenticationService>
					.Create()
					.RegisterDefaultSerializers()
					.RegisterJsonNetSerializer()
					.RegisterDotNetHttpClient(QueryForRemoteServiceEndpoint(serviceDiscovery, "Authentication"), new FiddlerEnabledWebProxyHandler())
					.Build();
			});

			register
				.RegisterType<InMemoryAuthDetailsModelRepository>()
				.As<IAuthDetailsRepository>()
				.SingleInstance();

			register
				.RegisterType<GuardiansUnityAuthenticationClient>()
				.As<IAuthenticationClient>()
				.SingleInstance();

			register.RegisterInstance(new UnityLogger(LogLevel.All))
				.As<ILog>()
				.SingleInstance();

			register.RegisterType<UnityLocalAuthDetailsValidator>()
				.As<IValidator<IUserAuthenticationDetailsContainer>>()
				.SingleInstance();

			register.RegisterType<AuthenticationTokenRepository>()
				.As<IReadonlyAuthTokenRepository>()
				.As<IAuthTokenRepository>()
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
