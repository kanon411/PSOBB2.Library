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


			builder.RegisterType<MovementSimulationTickable>()
				.As<IGameTickable>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<EntityDataChangeTickable>()
				.As<IGameTickable>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<ChatMessageHandlerTickable>()
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

			RegisterEntityMappableCollections(builder);

			//TODO: This is legacy
			builder.RegisterType<DefaultGameObjectToEntityMappable>()
				.As<IReadonlyGameObjectToEntityMappable>()
				.As<IGameObjectToEntityMappable>()
				.SingleInstance();

			//Auth token
			builder.RegisterType<AuthenticationTokenRepository>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<DefaultLoadableContentResourceManager>()
				.As<ILoadableContentResourceManager>()
				.As<IDisposable>()
				.AsSelf()
				.SingleInstance();

			//Text chat factory
			builder.RegisterType<DefaultTextChatEventFactory>()
				.As<ITextChatEventFactory>();

			//Text queue
			builder.RegisterType<Queue<TextChatEventData>>()
				.AsSelf()
				.SingleInstance();
		}

		//TODO: Refactor into module.
		private static void RegisterEntityMappableCollections(ContainerBuilder builder)
		{
			//The below is kinda a hack to register the non-generic types in the
			//removabale collection
			List<IEntityCollectionRemovable> removableComponentsList = new List<IEntityCollectionRemovable>(10);

			builder.RegisterGeneric(typeof(EntityGuidDictionary<>))
				.AsSelf()
				.As(typeof(IReadonlyEntityGuidMappable<>))
				.As(typeof(IEntityGuidMappable<>))
				.OnActivated(args =>
				{
					if(args.Instance is IEntityCollectionRemovable removable)
						removableComponentsList.Add(removable);
				})
				.SingleInstance();

			//This will allow everyone to register the removable collection collection.
			builder.RegisterInstance(removableComponentsList)
				.AsImplementedInterfaces()
				.AsSelf()
				.SingleInstance();
		}
	}
}
