using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
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
