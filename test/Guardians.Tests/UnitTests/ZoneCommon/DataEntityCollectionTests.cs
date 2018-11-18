using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Guardians.Tests.Collections
{
	[TestFixture]
	public sealed class DataEntityCollectionTests
	{
		[Test]
		public void Test_Can_Create_Ctor()
		{
			Assert.DoesNotThrow(() => new EntityFieldDataCollection<TestFieldType>());
		}
	}
}
