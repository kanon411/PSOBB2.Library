﻿using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using ProtoBuf;
using ProtoBuf.Meta;

namespace PSOBB
{
	class Program
	{
		static async Task Main(string[] args)
		{
			ProtobufNetGladNetSerializerAdapter serializer = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);

			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();

			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);

			var client = new DotNetTcpClientNetworkClient()
				.AddHeaderlessNetworkMessageReading(serializer)
				.For<GameServerPacketPayload, GameClientPacketPayload, IGamePacketPayload>()
				.Build()
				.AsManaged(new ConsoleLogger(LogLevel.All));

			await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 5006);
			Thread.Sleep(3000);

			Console.WriteLine("Enter Character ID for test:");

			int characterId = int.Parse(Console.ReadLine());

			await client.SendMessage(new ClientSessionClaimRequestPayload("Test", characterId));

			int packetCount = 0;

			try
			{
				while(true)
				{
					NetworkIncomingMessage<GameServerPacketPayload> message = await client.ReadMessageAsync()
						.ConfigureAwait(false);

					Console.WriteLine($"\n#{packetCount} - Recieved Message Type: {message.Payload.GetType().Name}");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
