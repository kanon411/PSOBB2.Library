using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

		public static IEnumerable<MemberInfo> SerializableMembers { get; }
			= ModelTypes
				.SelectMany(t => t.GetProperties().Select(p => (MemberInfo)p).Concat(t.GetFields()));

		[Test]
		[TestCaseSource(nameof(ModelTypes))]
		public void Test_Model_Has_Parameterless_Ctor(Type t)
		{
			//assert
			Assert.NotNull(t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, Enumerable.Empty<Type>().ToArray(), null), $"Type: {t.Name} does not have a required parameterless ctor. In Assembly: {t.Assembly.FullName}");
		}

		[Test]
		[TestCaseSource(nameof(SerializableMembers))]
		public void Test_Model_Properties_All_Have_Explict_Json_Properties(MemberInfo m)
		{
			//assert
			Assert.True(m.GetCustomAttribute<JsonPropertyAttribute>() != null || m.GetCustomAttribute<JsonIgnoreAttribute>() != null, $"Property: {m.Name} in Type: {m.DeclaringType.Name} does not contain explict {nameof(JsonProperty)} or {nameof(JsonIgnoreAttribute)}");
		}
	}
}
