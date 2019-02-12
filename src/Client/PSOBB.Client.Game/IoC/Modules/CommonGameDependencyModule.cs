using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autofac;

namespace PSOBB.Client
{
	public sealed class CommonGameDependencyModule : NetworkServiceDiscoveryableAutofaceModule
	{
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		private GameSceneType Scene { get; }

		/// <summary>
		/// Default autofac ctor.
		/// </summary>
		public CommonGameDependencyModule()
		{
			//We shouldn't call this, I don't think.
		}

		/// <inheritdoc />
		public CommonGameDependencyModule(GameSceneType scene)
		{
			if(!Enum.IsDefined(typeof(GameSceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(GameSceneType));
			Scene = scene;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Token module is needed in most scenes.
			builder.RegisterModule<AuthenticationTokenAutofacModule>();
			
			//Handlers aren't needed for all scenes, but for most.
			//TODO: We should expose SceneTypeCreatable or whatever on handlers
			builder.RegisterModule<GameClientMessageHandlerAutofacModule>();

			builder.RegisterModule(new EngineInterfaceRegisterationModule(Scene));
			builder.RegisterModule(new UIDependencyRegisterationModule());
		}
	}
}
