using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;
using NUnit.Framework;

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
			IContainer resolver = builder.Build();

			//arrange
			MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload> handler = resolver.Resolve<MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload>>();

			//assert
			Assert.NotNull(handler);
		}
	}
}
