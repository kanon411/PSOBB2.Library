using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using GaiaOnline;
using GladNet;
using Moq;
using NUnit.Framework;
using SceneJect;
using SceneJect.Common;

namespace Guardians
{
	[TestFixture]
	public class GeneralHandlerTests
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

				IGaiaOnlineImageCDNClient gaiaCdnClient = Mock.Of<IGaiaOnlineImageCDNClient>();
				IGaiaOnlineQueryClient gaiaQueryClient = Mock.Of<IGaiaOnlineQueryClient>();

				foreach(UnityUIRegisterationKey key in Enum.GetValues(typeof(UnityUIRegisterationKey)))
				{
					builder.RegisterInstance(uiText)
						.Keyed<IUIText>(key);

					builder.RegisterInstance(uiImage)
						.Keyed<IUIImage>(key);
				}

				builder.RegisterInstance(gaiaCdnClient)
					.As<IGaiaOnlineImageCDNClient>();
				builder.RegisterInstance(gaiaQueryClient)
					.As<IGaiaOnlineQueryClient>();
			}
		}

		[Test]
		public void Test_Can_Create_MessageHandlerService_From_DependencyModules()
		{
			//TODO: This won't work if we have multiple configurable modules.
			//arrange
			ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterAssemblyModules(typeof(ZoneClientHandlerRegisterationAutofacModule).Assembly);
			builder.RegisterModule(new MockedUIDependenciesAutofacModule());

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

			IContainer resolver = builder.Build();

			MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload> handler = null;

			//act
			try
			{
				handler = resolver.Resolve<MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload>>();
			}
			catch(DependencyResolutionException e)
			{
				//This makes it so the error is more readable. So we can see the exact dependency that is missing.
				DependencyResolutionException dependencyResolveException = e;

				while(dependencyResolveException.InnerException is DependencyResolutionException)
					dependencyResolveException = (DependencyResolutionException)dependencyResolveException.InnerException;

				Assert.Fail($"Failed: {dependencyResolveException.Message}\n\n{dependencyResolveException.StackTrace}");
			}
			

			//assert
			Assert.NotNull(handler);
		}
	}
}
