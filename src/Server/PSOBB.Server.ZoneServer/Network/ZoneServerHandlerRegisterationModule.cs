using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using GladNet;
using Module = Autofac.Module;

namespace PSOBB
{
	//TODO: Make a generic generalized version
	public sealed class ZoneServerHandlerRegisterationModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			IEnumerable<Type> handlerTypes = LoadHandlerTypes().ToArray();

			//Registers each type.
			foreach(Type t in handlerTypes)
				builder.RegisterType(t)
					.AsSelf()
					.SingleInstance();

			foreach(Type t in handlerTypes)
			{
				Type concretePayloadType = t.GetTypeInfo()
					.ImplementedInterfaces
					.First(i => i.GetTypeInfo().IsGenericType && i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,,>))
					.GetGenericArguments()
					.First();

				Type tryHandlerType = typeof(TrySemanticsBasedOnTypePeerMessageHandler<,,,>)
					.MakeGenericType(typeof(GameClientPacketPayload), typeof(GameServerPacketPayload), concretePayloadType, typeof(IPeerSessionMessageContext<GameServerPacketPayload>));

				builder.Register(context =>
					{
						object handler = context.Resolve(t);

						return Activator.CreateInstance(tryHandlerType, handler);
					})
					.As(typeof(IPeerMessageHandler<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>))
					.SingleInstance();
			}
		}

		private IEnumerable<Type> LoadHandlerTypes()
		{
			return GetType().GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,,>)) && !t.IsAbstract);
		}
	}
}
