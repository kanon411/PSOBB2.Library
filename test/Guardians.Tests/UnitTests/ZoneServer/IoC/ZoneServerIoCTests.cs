using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;
using Common.Logging.Simple;
using GladNet;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public sealed class ZoneServerIoCTests
	{
		[Test]
		public void Test_Can_Create_IGameTickables()
		{
			//arrange
			DefaultZoneServerDependencyRegistrar registrar = new DefaultZoneServerDependencyRegistrar(new NoOpLogger(), new NetworkAddressInfo(IPAddress.Any, 5000));
			ContainerBuilder builder = new ContainerBuilder();
			registrar.RegisterServices(builder);
			IContainer build = builder.Build();

			//assert
			Assert.DoesNotThrow(() => build.Resolve<IEnumerable<IGameTickable>>());
		}
	}
}
