using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
			Assert.DoesNotThrow(() => factory.Create(new ZoneServerApplicationBaseCreationContext(new NoOpLogger(), new NetworkAddressInfo(IPAddress.Any, 5000))));
		}
	}
}
