using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autofac;
using Common.Logging;
using Refit;

namespace PSOBB.Client
{
	public sealed class CommonGameDependencyModule : NetworkServiceDiscoveryableAutofaceModule
	{
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		private GameSceneType Scene { get; }

		private string ServiceDiscoveryUrl { get; }

		/// <summary>
		/// Default autofac ctor.
		/// </summary>
		public CommonGameDependencyModule()
		{
			//We shouldn't call this, I don't think.
		}

		//TODO: Shoudl we expose the ServiceDiscovery URL to the editor? Is there value in that?
		/// <inheritdoc />
		public CommonGameDependencyModule(GameSceneType scene, [NotNull] string serviceDiscoveryUrl = "http://192.168.0.3:5000")
		{
			if(!Enum.IsDefined(typeof(GameSceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(GameSceneType));

			Scene = scene;
			ServiceDiscoveryUrl = serviceDiscoveryUrl ?? throw new ArgumentNullException(nameof(serviceDiscoveryUrl));
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Set the sync context
			UnityExtended.InitializeSyncContext();

			builder.Register(context => LogLevel.All)
				.As<LogLevel>()
				.SingleInstance();

			builder.RegisterType<UnityLogger>()
				.As<ILog>()
				.SingleInstance();

			builder.RegisterType<LocalCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.SingleInstance();

			builder.RegisterType<AuthenticationTokenRepository>()
				.As<IAuthTokenRepository>()
				.As<IReadonlyAuthTokenRepository>()
				.SingleInstance();

			//Token module is needed in most scenes.
			builder.RegisterModule<AuthenticationTokenAutofacModule>();
			
			//Handlers aren't needed for all scenes, but for most.
			//TODO: We should expose SceneTypeCreatable or whatever on handlers
			builder.RegisterModule(new GameClientMessageHandlerAutofacModule(Scene));

			builder.RegisterModule(new EngineInterfaceRegisterationModule(Scene, GetType().Assembly));
			builder.RegisterModule(new UIDependencyRegisterationModule());

			builder.Register<IServiceDiscoveryService>(context => RestService.For<IServiceDiscoveryService>(ServiceDiscoveryUrl))
				.As<IServiceDiscoveryService>()
				.SingleInstance();
		}
	}
}
