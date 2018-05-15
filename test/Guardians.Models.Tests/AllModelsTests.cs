using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public class AllModelsTests
	{
		public static IEnumerable<Type> ModelTypes { get; } = AuthenticationModelsMetadataMarker.ModelTypes
			.Concat(GameServerModelsMetadataMarker.ModelTypes)
			.Concat(ServerSelectionModelsMetadataMarker.ModelTypes);

		[Test]
		[TestCaseSource(nameof(ModelTypes))]
		public void Test_Model_Has_Parameterless_Ctor(Type t)
		{
			//assert
			Assert.NotNull(t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, Enumerable.Empty<Type>().ToArray(), null), $"Type: {t.Name} does not have a required parameterless ctor. In Assembly: {t.Assembly.FullName}");
		}
	}
}
