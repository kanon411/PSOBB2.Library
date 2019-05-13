using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using UnityEngine;

namespace GladMMO
{
	public sealed class LobbyGameDependenciesAutofacModule : Module
	{
		public LobbyGameDependenciesAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<EntityPrefabFactory>()
				.As<IFactoryCreatable<GameObject, EntityPrefab>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<UtcNowNetworkTimeService>()
				.As<INetworkTimeService>()
				.As<IReadonlyNetworkTimeService>()
				.SingleInstance();

			//This service is required by the entity data change system/tickable
			builder.RegisterType<EntityDataChangeCallbackManager>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<DefaultLocalPlayerDetails>()
				.AsImplementedInterfaces()
				.SingleInstance();

			//TODO: This is legacy
			builder.RegisterType<DefaultGameObjectToEntityMappable>()
				.As<IReadonlyGameObjectToEntityMappable>()
				.As<IGameObjectToEntityMappable>()
				.SingleInstance();
		}
	}
}
