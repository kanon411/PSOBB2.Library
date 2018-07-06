using System;
using System.Linq;
using System.Net;
using System.Threading;
using GladNet;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Guardians
{
	class Program
	{
		static void Main(string[] args)
		{
			RuntimeTypeModel.Default.Add(typeof(GameClientPacketPayload), true);
			RuntimeTypeModel.Default.Add(typeof(GameServerPacketPayload), true);

			ZoneServerMetadataMarker.ClientPayloadTypesByOpcode
				.AsEnumerable()
				.Concat(ZoneServerMetadataMarker.ServerPayloadTypesByOpcode)
				.ToList()
				.ForEach(pair =>
				{
					RuntimeTypeModel.Default.Add(pair.Value, true);

					RuntimeTypeModel.Default[pair.Value.BaseType]
						.AddSubType((int)pair.Key, pair.Value);

				});

			ProtobufNetGladNetSerializerAdapter serializer = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);

			var client = new DotNetTcpClientNetworkClient()
				.AddHeaderlessNetworkMessageReading(serializer)
				.For<GameServerPacketPayload, GameClientPacketPayload, IGamePacketPayload>()
				.Build()
				.AsManaged();

			client.Connect(IPAddress.Parse("127.0.0.1"), 5006);

			Thread.Sleep(1000);

			client.SendMessage(new ClientSessionClaimRequestPayload("Test", 2));

			Thread.Sleep(5000);
		}
	}
}
