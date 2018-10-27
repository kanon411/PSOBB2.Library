using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;
using NUnit.Framework;
using SceneJect;
using SceneJect.Common;

namespace Guardians
{
	[TestFixture]
	public class GeneralHandlerTests
	{
		[Test]
		public void Test_Can_Create_MessageHandlerService_From_DependencyModules()
		{
			//TODO: This won't work if we have multiple configurable modules.
			//arrange
			ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterAssemblyModules(typeof(ZoneClientHandlerRegisterationAutofacModule).Assembly);

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

			//arrange
			MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload> handler = resolver.Resolve<MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload>>();

			//assert
			Assert.NotNull(handler);
		}
	}
}
