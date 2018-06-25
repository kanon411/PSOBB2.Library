using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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

			//Directly from ASP Core.
			//
			// ModelBinding, Validation
			//
			// The DefaultModelMetadataProvider does significant caching and should be a singleton.
			services.TryAddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>();
			services.TryAdd(ServiceDescriptor.Transient<ICompositeMetadataDetailsProvider>(s =>
			{
				//TODO: Is it ok to not use options?
				return new DefaultCompositeMetadataDetailsProvider(new List<IMetadataDetailsProvider>());
			}));
			services.TryAddSingleton<IModelBinderFactory, ModelBinderFactory>();
			services.TryAddSingleton<IObjectModelValidator>(s =>
			{
				//TODO: Is it ok to not use options?
				var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
				return new DefaultObjectValidator(metadataProvider, new List<IModelValidatorProvider>());
			});

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
					.RegisterDotNetHttpClient(QueryForRemoteServiceEndpoint(serviceDiscovery, "Authentication"))
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

			register.Populate(services);
		}

		private async Task<string> QueryForRemoteServiceEndpoint(IServiceDiscoveryService serviceDiscovery, string serviceType)
		{
			ResolveServiceEndpointResponse endpointResponse = await serviceDiscovery.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceType));

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for Service: {serviceType} Result: {endpointResponse.ResultCode}");

			Debug.Log($"Recieved service discovery response: {endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort} for Type: {serviceType}");

			//TODO: Do we need extra slash?
			return $"http://{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/";
		}
	}
}
