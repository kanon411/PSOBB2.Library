using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using ProtoBuf;
using SceneJect.Common;
using TypeSafe.Http.Net;
using UnityEngine;
using UnityEngine.Rendering;

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
			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.CheckCertificateRevocationList = false;

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
			RegisterGameTickable(builder);

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

			builder.RegisterType<MovementInformationCollection>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<MovementInformation>>()
				.As<IEntityGuidMappable<MovementInformation>>()
				.As<IDirtyableMovementInformationCollection>()
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

			builder.RegisterType<DefaultGameObjectToEntityMappable>()
				.As<IReadonlyGameObjectToEntityMappable>()
				.As<IGameObjectToEntityMappable>()
				.SingleInstance();

			RegisterPlayerFactoryServices(builder);

			builder.RegisterType<DefaultEntityFactory<DefaultEntityCreationContext>>()
				.AsImplementedInterfaces()
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

			//Sceneject stuff that handles GameObject injections
			builder.RegisterType<DefaultGameObjectFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<DefaultInjectionStrategy>()
				.As<IInjectionStrategy>()
				.SingleInstance();

			//This is for mapping connection IDs to the main controlled EntityGuid.
			builder.RegisterInstance(new ConnectionEntityMap())
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<PlayerEntityGuidEnumerable>()
				.As<IPlayerEntityGuidEnumerable>()
				.AsSelf();

			builder.RegisterType<MovementUpdateMessageSender>()
				.As<INetworkMessageSender<EntityMovementMessageContext>>()
				.AsSelf();

			builder.RegisterType<TypeSafeServiceDiscoveryServiceClient>()
				.As<IServiceDiscoveryService>()
				.WithParameter(new TypedParameter(typeof(string), @"http://localhost:5003"))
				.SingleInstance();

			builder.Register(context =>
			{
				IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

				return TypeSafeHttpBuilder<IZoneServerToGameServerClient>
					.Create()
					.RegisterJsonNetSerializer()
					.RegisterDefaultSerializers()
					.RegisterDotNetHttpClient(QueryForRemoteServiceEndpoint(serviceDiscovery, "GameServer"), new FiddlerEnabledWebProxyHandler())
					.Build();
			})
				.As<IZoneServerToGameServerClient>()
				.SingleInstance();
		}

		private static void RegisterPlayerFactoryServices(ContainerBuilder builder)
		{
			builder.RegisterType<DefaultEntityFactory<PlayerEntityCreationContext>>()
				.AsSelf()
				.SingleInstance();

			builder.Register(context =>
				{
					using(var scope = context.Resolve<ILifetimeScope>().BeginLifetimeScope(subBuilder =>
					{
						subBuilder.RegisterType<AdditionalServerRegisterationPlayerEntityFactoryDecorator>()
							.WithParameter(new TypedParameter(typeof(IFactoryCreatable<GameObject, PlayerEntityCreationContext>), context.Resolve<DefaultEntityFactory<PlayerEntityCreationContext>>()));
					}))
					{
						return scope.Resolve<AdditionalServerRegisterationPlayerEntityFactoryDecorator>();
					}
				})
				.As<IFactoryCreatable<GameObject, PlayerEntityCreationContext>>()
				.SingleInstance();

			builder.RegisterType<EntityPrefabFactory>()
				.As<IFactoryCreatable<GameObject, EntityPrefab>>()
				.SingleInstance();
		}

		//TODO: Put this in a base class or something
		private async Task<string> QueryForRemoteServiceEndpoint(IServiceDiscoveryService serviceDiscovery, string serviceType)
		{
			ResolveServiceEndpointResponse endpointResponse = await serviceDiscovery.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceType));

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for Service: {serviceType} Result: {endpointResponse.ResultCode}");

			Debug.Log($"Recieved service discovery response: {endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort} for Type: {serviceType}");

			//TODO: Do we need extra slash?
			return $"{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/";
		}

		//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
		private bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}

		private static void RegisterGameTickable(ContainerBuilder builder)
		{
			builder.RegisterType<DefaultInterestRadiusManager>()
				.AsSelf()
				.As<IInterestRadiusManager>()
				.As<IGameTickable>()
				.SingleInstance();

			builder.RegisterType<PlayerEntityEntryManager>()
				.AsSelf()
				.As<IGameTickable>()
				.SingleInstance();

			builder.RegisterType<PlayerEntityMovementDataUpdateManager>()
				.AsSelf()
				.As<IGameTickable>()
				.SingleInstance();
		}
	}
}
