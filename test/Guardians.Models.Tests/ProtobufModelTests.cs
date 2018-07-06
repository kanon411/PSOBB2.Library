using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using ProtoBuf;

namespace Guardians
{
	[TestFixture]
	public sealed class ProtobufModelTests : GenericModelTests<ProtoMemberAttribute, ProtoIgnoreAttribute, ProtoContractAttribute>
	{
		public static IEnumerable<Type> AllProtobufPayloadModels => ZoneServerMetadataMarker.PayloadTypes;

		[Test]
		public void Test_No_Duplicate_OpCodes_For_Same_PayloadType()
		{
			//arrange
			IEnumerable<Type> distinctPayloads = ModelTypes
				.Distinct(new PayloadTypeEqualityComparerByOpCodeAndType())
				.ToArray();

			//assert
			Assert.AreEqual(ModelTypes.Count(), distinctPayloads.Count(), $"Encountered duplicate payload type More than 1 payload shares same OpCode and BaseType. Violating Types: {ModelTypes.Except(distinctPayloads).Aggregate("", (s, type) => $"{s} {type.Name}:OpCode.{type.GetCustomAttribute<GamePayloadAttribute>().OperationCode}")}");
		}

		[Test]
		[TestCaseSource(nameof(AllProtobufPayloadModels))]
		public void Test_All_Payloads_Have_GamePayloadAttribute(Type payloadType)
		{
			//assert
			Assert.True(payloadType.GetCustomAttribute<GamePayloadAttribute>() != null, $"Payload is missing {nameof(GamePayloadAttribute)}. Type: {payloadType.Name}");
		}

		private class PayloadTypeEqualityComparerByOpCodeAndType : EqualityComparer<Type>
		{
			/// <inheritdoc />
			public override bool Equals(Type x, Type y)
			{
				return x.BaseType == y.BaseType && x.GetCustomAttribute<GamePayloadAttribute>().OperationCode == y.GetCustomAttribute<GamePayloadAttribute>().OperationCode;
			}

			/// <inheritdoc />
			public override int GetHashCode(Type obj)
			{
				return $"{obj.BaseType.Name}{obj.GetCustomAttribute<GamePayloadAttribute>().OperationCode}".GetHashCode();
			}
		}
	}
}
