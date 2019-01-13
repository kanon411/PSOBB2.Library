﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Common.Logging;
using Dissonance.Networking;
using Fasterflect;
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

				IUIFillableImage fillableImage = Mock.Of<IUIFillableImage>();


				foreach(UnityUIRegisterationKey key in Enum.GetValues(typeof(UnityUIRegisterationKey)))
				{
					builder.RegisterInstance(uiText)
						.Keyed<IUIText>(key);

					builder.RegisterInstance(uiImage)
						.Keyed<IUIImage>(key);

					builder.RegisterInstance(uiButton)
						.Keyed<IUIButton>(key);

					builder.RegisterInstance(fillableImage)
						.Keyed<IUIFillableImage>(key);
				}

			}
		}

		public static ContainerBuilder CreateDefaultContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			//We can't let autofac do this, since we have multiple versions of some modules.
			foreach(var module in typeof(ZoneClientHandlerRegisterationAutofacModule).Assembly
				.GetExportedTypes()
				.Where(t => t.Inherits(typeof(Autofac.Module)))
				.Where(t => !t.IsAbstract) //we can't deal with base abstract autofac modules
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
			builder.Register(context => new DefaultGameObjectFactory(context.Resolve<ILifetimeScope>(), new DefaultInjectionStrategy()))
				.As<IGameObjectFactory>()
				.SingleInstance();

			builder.Register(context => new DefaultGameObjectComponentAttachmentFactory(context.Resolve<ILifetimeScope>(), new DefaultInjectionStrategy()))
				.As<IGameObjectComponentAttachmentFactory>()
				.SingleInstance();

			builder.Register(context => new DefaultManualInjectionStrategy(context.Resolve<IComponentContext>()))
				.As<IManualInjectionStrategy>()
				.SingleInstance();

			RegisterExpectedInSceneDependecyMocks(builder);

			return builder;
		}

		public static void RegisterExpectedInSceneDependecyMocks(ContainerBuilder builder)
		{
			builder.RegisterInstance(Mock.Of<IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext>>())
				.As<IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext>>();

			builder.RegisterInstance(Mock.Of<ISceneManager>())
				.As<ISceneManager>();

			builder.RegisterInstance(Mock.Of<ICommsNetworkState>())
				.As<ICommsNetworkState>();

			builder.RegisterInstance(Mock.Of<IPlayerTrackingRegisterable>())
				.As<IPlayerTrackingRegisterable>();

			//Common thing used in temp registeration
			builder.RegisterInstance(NetworkEntityGuid.Empty)
				.AsSelf();

			//We do this to override the UnityLogger which calls ECall Sec errors.
			builder.RegisterType<ConsoleLogger>()
				.AsSelf()
				.As<ILog>()
				.SingleInstance();

			builder.RegisterInstance(Mock.Of<IChatMessageBoxReciever>())
				.AsSelf()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
