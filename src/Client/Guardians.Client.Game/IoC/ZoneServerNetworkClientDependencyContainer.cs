using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using GladNet;
using ProtoBuf;
using SceneJect.Common;
using Sirenix.Serialization;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Module responsible for registering the dependencies associated with the game client.
	/// </summary>
	public sealed class ZoneServerNetworkClientDependencyContainer : NonBehaviourDependency
	{
		[SerializeField]
		private LogLevel LoggingLevel;

		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			ProtobufNetGladNetSerializerAdapter serializer = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);

			IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> client = new DotNetTcpClientNetworkClient()
				.AddHeaderlessNetworkMessageReading(serializer)
				.For<GameServerPacketPayload, GameClientPacketPayload, IPacketPayload>()
				.Build()
				.AsManaged(new UnityLogger(LoggingLevel));

			register.RegisterInstance(client)
				.As<IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload>>()
				.As<IPeerPayloadSendService<GameClientPacketPayload>>()
				.As<IPayloadInterceptable>()
				.As<IConnectionService>();

			register.RegisterType<DefaultMessageContextFactory>()
				.As<IPeerMessageContextFactory>()
				.SingleInstance();

			register.RegisterType<PayloadInterceptMessageSendService<GameClientPacketPayload>>()
				.As<IPeerRequestSendService<GameClientPacketPayload>>()
				.SingleInstance();
		}
	}
}
