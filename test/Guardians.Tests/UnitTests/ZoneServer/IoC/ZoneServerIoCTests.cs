using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;
using Autofac.Core;
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
			try
			{
				build.Resolve<IEnumerable<IGameTickable>>();
			}
			catch(DependencyResolutionException e)
			{
				//This makes it so the error is more readable. So we can see the exact dependency that is missing.
				DependencyResolutionException dependencyResolveException = e;

				while(dependencyResolveException.InnerException is DependencyResolutionException)
					dependencyResolveException = (DependencyResolutionException)dependencyResolveException.InnerException;

				Assert.Fail($"Failed: {dependencyResolveException.Message}\n\n{dependencyResolveException.StackTrace}");
			}
		}
	}
}
