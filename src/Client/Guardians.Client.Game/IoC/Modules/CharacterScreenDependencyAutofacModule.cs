using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Autofac;
using Common.Logging;
using Refit;

namespace Guardians
{
	public sealed class CharacterScreenDependencyAutofacModule : NetworkServiceDiscoveryableAutofaceModule
	{
		private string ServiceDiscoveryUrl { get; }

		/// <inheritdoc />
		public CharacterScreenDependencyAutofacModule(string serviceDiscoveryUrl)
		{
			if(string.IsNullOrEmpty(serviceDiscoveryUrl)) throw new ArgumentException("Value cannot be null or empty.", nameof(serviceDiscoveryUrl));
			ServiceDiscoveryUrl = serviceDiscoveryUrl;
		}

		public CharacterScreenDependencyAutofacModule()
		{
			//Default URL
			ServiceDiscoveryUrl = @"http://localhost:80";
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			ContainerBuilder register = builder;

			//Set the sync context
			UnityExtended.InitializeSyncContext();

			//Postsharp requires we setup some backend stuff
			//CachingServices.DefaultBackend = new MemoryCachingBackend();

			register.RegisterInstance(new UnityLogger(LogLevel.All))
				.As<ILog>()
				.SingleInstance();

			register.RegisterType<AuthenticationTokenRepository>()
				.As<IReadonlyAuthTokenRepository>()
				.As<IAuthTokenRepository>()
				.SingleInstance();

			register.Register<IServiceDiscoveryService>(context => RestService.For<IServiceDiscoveryService>(ServiceDiscoveryUrl))
				.As<IServiceDiscoveryService>()
				.SingleInstance();

			register.Register(context =>
			{
				//The below is not true for right now, we have global service discovery point to the gameserver for testing.
				//This registeration is abit complicated
				//because we are skipping the game server selection
				//to do this we must query the service discovery and THEN
				//we query the the gameserver's service discovery.
				IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

				return new AsyncEndpointCharacterService(QueryForRemoteServiceEndpoint(serviceDiscovery, "GameServer"));
			})
				.As<ICharacterService>()
				.SingleInstance();

			//Name query service
			//TODO: We should hand;e this differently
			register.Register(context => new CachedNameQueryServiceDecorator(new RemoteNetworkedNameQueryService(context.Resolve<ICharacterService>())))
				.As<INameQueryService>()
				.SingleInstance();

			register.RegisterType<LocalCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.SingleInstance();
		}
	}
}
