﻿using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using NUnit.Framework;
using ProtoBuf;
using UnityEngine;

namespace PSOBB
{
	[TestFixture]
	public sealed class PayloadSerializationTests
	{
		[Test]
		public void Test_Can_Serializer_MovementPacket_With_PathMovementData()
		{
			//arrange
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);
			GladNet.ProtobufNetGladNetSerializerAdapter gladnetProtobuf = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);

			//act
			MovementDataUpdateEventPayload payload = new MovementDataUpdateEventPayload(new AssociatedMovementData[]
			{
				new AssociatedMovementData(new NetworkEntityGuid(55), new PathBasedMovementData(new Vector3[]
				{
					new Vector3(1,2,3),
					new Vector3(2,3,4) 
				}, 55)), 
			});

			byte[] serialize = gladnetProtobuf.Serialize(payload);

			//assert
			Assert.NotNull(serialize);
			Assert.IsNotEmpty(serialize);
		}
	}
}
