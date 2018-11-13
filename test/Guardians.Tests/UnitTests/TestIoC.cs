using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Fasterflect;
using GaiaOnline;
using Moq;
using SceneJect;
using SceneJect.Common;

namespace Guardians
{
	internal static class TestIoC
	{
		//TODO: Refactor and extract this
		public class MockedUIDependenciesAutofacModule : Module
		{
			public MockedUIDependenciesAutofacModule()
			{

			}

			/// <inheritdoc />
			protected override void Load(ContainerBuilder builder)
			{
				//TODO: Automate discovery of adapter types
				IUIText uiText = Mock.Of<IUIText>();
				IUIImage uiImage = Mock.Of<IUIImage>();
				IUIButton uiButton = Mock.Of<IUIButton>();

				IGaiaOnlineImageCDNClient gaiaCdnClient = Mock.Of<IGaiaOnlineImageCDNClient>();
				IGaiaOnlineQueryClient gaiaQueryClient = Mock.Of<IGaiaOnlineQueryClient>();

				foreach(UnityUIRegisterationKey key in Enum.GetValues(typeof(UnityUIRegisterationKey)))
				{
					builder.RegisterInstance(uiText)
						.Keyed<IUIText>(key);

					builder.RegisterInstance(uiImage)
						.Keyed<IUIImage>(key);

					builder.RegisterInstance(uiButton)
						.Keyed<IUIButton>(key);
				}

				builder.RegisterInstance(gaiaCdnClient)
					.As<IGaiaOnlineImageCDNClient>();
				builder.RegisterInstance(gaiaQueryClient)
					.As<IGaiaOnlineQueryClient>();
			}
		}

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
			builder.RegisterModule(new MockedUIDependenciesAutofacModule());

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
