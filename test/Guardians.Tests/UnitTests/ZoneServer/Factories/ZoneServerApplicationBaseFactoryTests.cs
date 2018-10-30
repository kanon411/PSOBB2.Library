using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac.Core;
using Common.Logging.Simple;
using GladNet;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public sealed class ZoneServerApplicationBaseFactoryTests
	{
		[Test]
		public void Test_Can_Create_ZoneServerAppBaseFactory()
		{
			//assert
			Assert.DoesNotThrow(() => new DefaultZoneServerApplicationBaseFactory());
		}

		[Test]
		public void Test_Can_Create_ZoneServerAppBase_Without_Throwing()
		{
			//arrange
			DefaultZoneServerApplicationBaseFactory factory = new DefaultZoneServerApplicationBaseFactory();

			//assert
			try
			{
				factory.Create(new ZoneServerApplicationBaseCreationContext(new NoOpLogger(), new NetworkAddressInfo(IPAddress.Any, 5000))));
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
