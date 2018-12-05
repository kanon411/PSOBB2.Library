using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using UnityEngine;

namespace Guardians
{
	public sealed class ZoneServerGameDependenciesAutofacModule : Module
	{
		public ZoneServerGameDependenciesAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			//Set the sync context
			UnityExtended.InitializeSyncContext();

			builder.RegisterType<EntityGuidDictionary<IMovementData>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<IMovementData>>()
				.As<IEntityGuidMappable<IMovementData>>()
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

			builder.RegisterType<DefaultEntityFactory<DefaultEntityCreationContext>>()
				.As<IFactoryCreatable<GameObject, DefaultEntityCreationContext>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<EntityPrefabFactory>()
				.As<IFactoryCreatable<GameObject, EntityPrefab>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<DefaultEntityDestructor>()
				.As<IObjectDestructorable<NetworkEntityGuid>>()
				.AsSelf();

			builder.RegisterType<UtcNowNetworkTimeService>()
				.As<INetworkTimeService>()
				.As<IReadonlyNetworkTimeService>()
				.SingleInstance();

			builder.RegisterType<DefaultMovementHandlerService>()
				.As<IMovementDataHandlerService>()
				.AsSelf();

			builder.RegisterType<PositionChangeMovementBlockHandler>()
				.As<IMovementBlockHandler>()
				.AsSelf();

			builder.RegisterType<PathMovementBlockHandler>()
				.As<IMovementBlockHandler>()
				.AsSelf();

			builder.RegisterType<PositionChangeMovementWithLookBlockHandler>()
				.As<IMovementBlockHandler>()
				.AsSelf();

			builder.RegisterType<PositionChangeMovementDataDefaultVRBlockHandler>()
				.As<IMovementBlockHandler>()
				.AsSelf();

			builder.RegisterType<EntityGuidDictionary<IMovementGenerator<GameObject>>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<IMovementGenerator<GameObject>>>()
				.As<IEntityGuidMappable<IMovementGenerator<GameObject>>>()
				.SingleInstance();

			builder.RegisterType<MovementSimulationTickable>()
				.As<IGameTickable>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<EntityDataChangeTickable>()
				.As<IGameTickable>()
				.AsSelf()
				.SingleInstance();

			//This service is required by the entity data change system/tickable
			builder.RegisterType<EntityDataChangeCallbackManager>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<DefaultLocalPlayerDetails>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<LocalCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.SingleInstance();

			//TODO: Is this the best way to deal with this?
			builder.RegisterType<EntityGuidDictionary<IEntityDataFieldContainer>>()
				.As<IEntityGuidMappable<IEntityDataFieldContainer>>()
				.As<IReadonlyEntityGuidMappable<IEntityDataFieldContainer>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<EntityGuidDictionary<IChangeTrackableEntityDataCollection>>()
				.As<IEntityGuidMappable<IChangeTrackableEntityDataCollection>>()
				.As<IReadonlyEntityGuidMappable<IChangeTrackableEntityDataCollection>>()
				.AsSelf()
				.SingleInstance();

			//Auth token
			builder.RegisterType<AuthenticationTokenRepository>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
