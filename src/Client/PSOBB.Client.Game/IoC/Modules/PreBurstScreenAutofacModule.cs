﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;
using Refit;

namespace PSOBB
{
	public sealed class PreBurstScreenAutofacModule : NetworkServiceDiscoveryableAutofaceModule
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			//for legacy reasons
			ContainerBuilder register = builder;

			base.Load(builder);

			register.Register(context =>
				{
					//The below is not true for right now, we have global service discovery point to the gameserver for testing.
					//This registeration is abit complicated
					//because we are skipping the game server selection
					//to do this we must query the service discovery and THEN
					//we query the the gameserver's service discovery.
					IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

					return new AsyncEndpointCharacterService(QueryForRemoteServiceEndpoint(serviceDiscovery, "GameServer"), new RefitSettings() { HttpMessageHandlerFactory = () => new FiddlerEnabledWebProxyHandler()});
				})
				.As<ICharacterService>()
				.SingleInstance();

			register.Register(context =>
				{
					//The below is not true for right now, we have global service discovery point to the gameserver for testing.
					//This registeration is abit complicated
					//because we are skipping the game server selection
					//to do this we must query the service discovery and THEN
					//we query the the gameserver's service discovery.
					IServiceDiscoveryService serviceDiscovery = context.Resolve<IServiceDiscoveryService>();

					return new AsyncEndpointZoneServerService(QueryForRemoteServiceEndpoint(serviceDiscovery, "GameServer"));
				})
				.As<IZoneServerService>()
				.SingleInstance();
		}
	}
}
