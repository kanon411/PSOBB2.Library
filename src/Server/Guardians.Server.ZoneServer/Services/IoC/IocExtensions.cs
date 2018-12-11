using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using Fasterflect;

namespace Guardians
{
	public static class IocExtensions
	{
		public static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterTickableType<TTIckableType>(this ContainerBuilder builder)
			where TTIckableType : IGameTickable
		{
			return builder.RegisterType(typeof(TTIckableType));
		}

		/// <summary>
		/// Registeres a <see cref="IGameTickable"/>
		/// </summary>
		/// <typeparam name="TLimit"></typeparam>
		/// <typeparam name="TActivatorData"></typeparam>
		/// <typeparam name="TRegistrationStyle"></typeparam>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> AsGameTickable<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
		{
			builder
				.As<IGameTickable>()
				.OnActivating(args =>
				{
					//On activiation we need to handle the AOP attributes and decorate the tickable.
					args.ReplaceInstance(DecorateGameTickable(args.Instance as IGameTickable, args.Context));
				});

			return builder;
		}

		private static IGameTickable DecorateGameTickable([NotNull] IGameTickable gameTickable, [NotNull] IComponentContext argsContext)
		{
			if(gameTickable == null) throw new ArgumentNullException(nameof(gameTickable));
			if(argsContext == null) throw new ArgumentNullException(nameof(argsContext));

			IGameTickable originalGameTickable = gameTickable;

			//First let's checking locking attributes, since they are the most important
			CollectionsLockingAttribute lockingAttribute = gameTickable.GetType().GetCustomAttribute<CollectionsLockingAttribute>();
			if(lockingAttribute != null)
			{
				gameTickable = new GlobalCollectionsLockingPolicyTickableDecorator(argsContext.Resolve<GlobalEntityCollectionsLockingPolicy>(), gameTickable, lockingAttribute.DesiredLockingType);
			}

			//Here we can handle skippables, this was introduced to reduce write lock contention.
			if(originalGameTickable is ITickableSkippable skippable)
				gameTickable = new SkippableTickableDecorator(gameTickable, skippable);

			return gameTickable;
		}
	}
}
