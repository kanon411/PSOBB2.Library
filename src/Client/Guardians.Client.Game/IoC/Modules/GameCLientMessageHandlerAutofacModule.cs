using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace Guardians
{
	public sealed class GameClientMessageHandlerAutofacModule : Module
	{
		public GameClientMessageHandlerAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			//New IPeerContext generic param now so we register as implemented interface
			builder.RegisterType<ZoneClientDefaultRequestHandler>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload>>()
				.As<MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload>>();

			builder.RegisterModule<ZoneClientHandlerRegisterationAutofacModule>();
		}
	}
}
