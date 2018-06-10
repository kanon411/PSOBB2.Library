using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Autofac;
using Common.Logging;
using Common.Logging.Simple;
using GladNet;
using ProtoBuf;

namespace Guardians
{
	public sealed class ZoneServerApplicationBase : TcpServerServerApplicationBase<GameServerPacketPayload, GameClientPacketPayload>
	{
		private INetworkSerializationService Serializer { get; }

		private IContainer ServiceContainer { get; }

		/// <inheritdoc />
		public ZoneServerApplicationBase(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{
			if(serverAddress == null) throw new ArgumentNullException(nameof(serverAddress));

			Serializer = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);
			ServiceContainer = BuildServiceContainer();
		}

		private IContainer BuildServiceContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterInstance(Serializer)
				.As<INetworkSerializationService>();

			builder.RegisterType<ZoneServerDefaultRequestHandler>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterInstance(Logger)
				.As<ILog>();

			builder.RegisterType<MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>>()
				.As<MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>>()
				.SingleInstance();

			//This registers all the authentication message handlers
			builder.RegisterModule<ZoneServerHandlerRegisterationModule>();

			return builder.Build();
		}

		/// <inheritdoc />
		public ZoneServerApplicationBase(NetworkAddressInfo serverAddress, INetworkMessageDispatchingStrategy<GameServerPacketPayload, GameClientPacketPayload> messageHandlingStrategy) 
			: base(serverAddress, messageHandlingStrategy, new NoOpLogger())
		{
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(TcpClient tcpClient)
		{
			//TODO: This is where you would reject clients if you had a reason to.
			return true;
		}

		/// <inheritdoc />
		protected override IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> CreateIncomingSessionPipeline(TcpClient client)
		{
			return new DotNetTcpClientNetworkClient(client)
				.AddHeaderlessNetworkMessageReading(Serializer)
				.Build<GameClientPacketPayload, GameServerPacketPayload, IPacketPayload>()
				.AsManagedSession(Logger);
		}

		/// <inheritdoc />
		protected override ManagedClientSession<GameServerPacketPayload, GameClientPacketPayload> CreateIncomingSession(IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> client, SessionDetails details)
		{
			throw new NotImplementedException($"Client session is not implemented yet.");
		}
	}
}