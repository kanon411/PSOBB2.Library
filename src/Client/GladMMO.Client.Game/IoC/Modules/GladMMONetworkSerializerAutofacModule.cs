using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using FreecraftCore;
using GladNet;
using FreecraftCore.Serializer;

namespace GladMMO
{
	public sealed class GladMMONetworkSerializerAutofacModule : Module
	{
		public GladMMONetworkSerializerAutofacModule()
		{

		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SerializerService>()
				.AsSelf()
				.As<ISerializerService>()
				.OnActivated(args =>
				{
					//TODO: Automate discovery of payload types.
					args.Instance.RegisterType<GamePacketPayload>();
					args.Instance.RegisterType<ServerPacketHeader>();
					args.Instance.RegisterType<OutgoingClientPacketHeader>();
					args.Instance.RegisterType<SessionAuthChallengeEvent>();
					args.Instance.RegisterType<SessionAuthProofRequest>();
					args.Instance.RegisterType<AuthenticateSessionResponse>();
					args.Instance.RegisterType<CharacterLoginRequest>();
					args.Instance.RegisterType<CharacterListRequest>();
					args.Instance.RegisterType<CharacterListResponse>();
					args.Instance.RegisterType<SMSG_LOGIN_VERIFY_WORLD_PAYLOAD>();
					args.Instance.RegisterType<SMSG_COMPRESSED_UPDATE_OBJECT_Payload>();
					args.Instance.RegisterType<ChatMessageRequest>();

					args.Instance.Compile();
				})
				.SingleInstance();

			builder.RegisterType<FreecraftCoreGladNetSerializerAdapter>()
				.AsSelf()
				.As<INetworkSerializationService>()
				.SingleInstance();
		}
	}
}