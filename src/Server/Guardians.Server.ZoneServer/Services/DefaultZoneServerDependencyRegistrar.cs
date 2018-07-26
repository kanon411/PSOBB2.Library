using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Autofac;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	public sealed class DefaultZoneServerDependencyRegistrar
	{
		private ILog LoggerToRegister { get; }

		private NetworkAddressInfo AddressInfo { get; }

		/// <inheritdoc />
		public DefaultZoneServerDependencyRegistrar([NotNull] ILog loggerToRegister, [NotNull] NetworkAddressInfo addressInfo)
		{
			LoggerToRegister = loggerToRegister ?? throw new ArgumentNullException(nameof(loggerToRegister));
			AddressInfo = addressInfo ?? throw new ArgumentNullException(nameof(addressInfo));
		}

		public void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterInstance(new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32))
				.As<INetworkSerializationService>();

			builder.RegisterType<ZoneServerDefaultRequestHandler>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterInstance(LoggerToRegister)
				.As<ILog>();

			builder.RegisterType<MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>>()
				.As<MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>>()
				.SingleInstance();

			//This registers all the authentication message handlers
			builder.RegisterModule<ZoneServerHandlerRegisterationModule>();

			builder.RegisterInstance(new DefaultSessionCollection())
				.As<IRegisterable<int, ZoneClientSession>>()
				.As<ISessionCollection>()
				.SingleInstance();

			builder.RegisterInstance(AddressInfo)
				.As<NetworkAddressInfo>()
				.SingleInstance()
				.ExternallyOwned();

			builder.RegisterType<ZoneServerApplicationBase>()
				.SingleInstance()
				.AsSelf();

			//We also have to register factories now for session/sessionclient
			builder.RegisterType<DefaultManagedClientSessionFactory>()
				.AsImplementedInterfaces();

			builder.RegisterType<DefaultManagedSessionFactory>()
				.AsImplementedInterfaces();

			//TODO: Extract this into a handlers registrar
			//We have some entity services that require being registered
			builder.RegisterType<QueueBasedPlayerEntitySessionGateway>()
				.AsImplementedInterfaces()
				.SingleInstance(); //must only be one, since it's basically a collection.
		}
	}
}
