using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Fasterflect;
using SceneJect;
using SceneJect.Common;

namespace Guardians
{
	internal static class TestIoC
	{
		public static ContainerBuilder CreateDefaultContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			//We can't let autofac do this, since we have multiple versions of some modules.
			foreach(var module in typeof(ZoneClientHandlerRegisterationAutofacModule).Assembly
				.GetExportedTypes()
				.Where(t => t.Inherits(typeof(Autofac.Module)))
				.Where(t => t != typeof(GameInitializableRegisterationAutofacModule))) //TODO: Make this so we can override the modules better
			{
				builder.RegisterModule(Activator.CreateInstance(module) as Module);
			}
			builder.RegisterModule(new GeneralHandlerTests.MockedUIDependenciesAutofacModule());

			//Register every GameInit module
			foreach(GameInitializableSceneSpecificationAttribute.SceneType sceneType in Enum.GetValues(typeof(GameInitializableSceneSpecificationAttribute.SceneType)))
			{
				builder.RegisterModule(new GameInitializableRegisterationAutofacModule(sceneType));
			}

			//Manually register SceneJect services
			builder.Register(context => new DefaultGameObjectFactory(context.Resolve<IComponentContext>(), new DefaultInjectionStrategy()))
				.As<IGameObjectFactory>()
				.SingleInstance();

			builder.Register(context => new DefaultGameObjectComponentAttachmentFactory(context.Resolve<IComponentContext>(), new DefaultInjectionStrategy()))
				.As<IGameObjectComponentAttachmentFactory>()
				.SingleInstance();

			builder.Register(context => new DefaultManualInjectionStrategy(context.Resolve<IComponentContext>()))
				.As<IManualInjectionStrategy>()
				.SingleInstance();

			return builder;
		}
	}
}
