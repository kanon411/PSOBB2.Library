using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Autofac;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using ProtoBuf;
using UnityEngine;

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

			//gametickables
			builder.RegisterType<DefaultInterestRadiusManager>()
				.AsSelf()
				.As<IGameTickable>()
				.SingleInstance();

			builder.RegisterType<PlayerEntityEntryManager>()
				.AsSelf()
				.As<IGameTickable>()
				.SingleInstance();

			//tickable services (may be shared with handlers)
			builder.RegisterType<EntityGuidDictionary<InterestCollection>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<InterestCollection>>()
				.As<IEntityGuidMappable<InterestCollection>>()
				.SingleInstance(); //only 1, else we will get problems

			//TODO: We should automate the registeration of message senders
			builder.RegisterType<VisibilityChangeMessageSender>()
				.AsImplementedInterfaces()
				.AsSelf();

			builder.RegisterType<EntityGuidDictionary<MovementInformation>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<MovementInformation>>()
				.As<IEntityGuidMappable<MovementInformation>>()
				.SingleInstance();

			builder.RegisterType<EntityGuidDictionary<IPeerPayloadSendService<GameServerPacketPayload>>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>>>()
				.As<IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>>>()
				.SingleInstance();

			builder.RegisterType<EntityGuidDictionary<GameObject>>()
				.As<IEntityGuidMappable<GameObject>>()
				.As<IReadonlyEntityGuidMappable<GameObject>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<PlayerEntityFactory>()
				.As<IFactoryCreatable<GameObject, PlayerEntityCreationContext>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<GenericMessageSender<PlayerSelfSpawnEventPayload>>()
				.AsSelf()
				.AsImplementedInterfaces();

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
