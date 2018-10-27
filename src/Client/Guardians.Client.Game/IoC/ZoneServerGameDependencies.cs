using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	public sealed class ZoneServerGameDependencies : NonBehaviourDependency
	{
		/// <inheritdoc />
		public override void Register(ContainerBuilder builder)
		{
			builder.RegisterType<EntityGuidDictionary<MovementInformation>>()
				.AsSelf()
				.As<IReadonlyEntityGuidMappable<MovementInformation>>()
				.As<IEntityGuidMappable<MovementInformation>>()
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

			builder.RegisterType<LocalPlayerFactory>()
				.As<IFactoryCreatable<GameObject, LocalPlayerCreationContext>>()
				.AsSelf()
				.SingleInstance();
		}
	}
}
