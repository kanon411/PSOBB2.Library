using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
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
			Assert.True(m.GetCustomAttribute<JsonPropertyAttribute>() != null || m.GetCustomAttribute<JsonIgnoreAttribute>() != null, $"{GenerateMemberInfoIdentiferPrefix(m)} does not contain explict {nameof(JsonProperty)} or {nameof(JsonIgnoreAttribute)}");
		}

		private static string GenerateMemberInfoIdentiferPrefix(MemberInfo m)
		{
			return $"Property: {m.Name} in Type: {m.DeclaringType.Name}";
		}

		[Test]
		[TestCaseSource(nameof(SerializableMembers))]
		public void Test_Model_No_Array_Directly_Exposed(MemberInfo m)
		{
			//assert
			Assert.False(m.GetUnderlyingType().IsArray && m.IsPublic(), $"{GenerateMemberInfoIdentiferPrefix(m)} is an Array type but was public. Do not expose arrays publicly. Use them as backing fields to collections.");
		}

		[Test]
		[TestCaseSource(nameof(SerializableMembers))]
		public void Test_Model_Any_Public_Collections_Are_Readonly(MemberInfo m)
		{
			//assert
			Assert.False(!m.GetUnderlyingType().IsArray && m.IsPublic() && m.GetUnderlyingType() != typeof(string) && (TypeIsReadonlyCollection(m)), $"{GenerateMemberInfoIdentiferPrefix(m)} is a collection type that is not readonly. It uses Type: {m.GetUnderlyingType()}. Use IReadonlyCollection<T>. Do not use IEnumerable<T>.");
		}

		private static bool TypeIsReadonlyCollection(MemberInfo m)
		{
			IEnumerable<Type> interfaces = m.GetUnderlyingType()
				.GetInterfaces();

			if(m.GetUnderlyingType().IsInterface)
				interfaces = interfaces.Concat(new Type[1] {m.GetUnderlyingType()});

			return interfaces
				.Any(i => (i == typeof(ICollection) || i == typeof(IList))
					|| i.IsGenericType 
					&& (i.GetGenericTypeDefinition() == typeof(ICollection<>)
					|| i.GetGenericTypeDefinition() == typeof(IList<>)));
		}
	}

	public static class AllModelsTestsHelper
	{
		public static Type GetUnderlyingType(this MemberInfo member)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				default:
					throw new ArgumentException
					(
						"Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
					);
			}
		}

		public static bool IsPublic([NotNull] this MemberInfo member)
		{
			if(member == null) throw new ArgumentNullException(nameof(member));

			switch(member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo)member).IsPublic;
				case MemberTypes.Method:
					return ((MethodInfo)member).IsPublic;
				//TODO: Is that the best way to do this?
				case MemberTypes.Property:
					return ((PropertyInfo)member).GetMethod.IsPublic;
				default:
					throw new InvalidOperationException($"Cannot find accessibility information for MemberType: {member.MemberType}.");
			}
		}
	}
}
