using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using GladNet;
using UnityEngine;
using Module = Autofac.Module;

namespace Guardians
{
	//based on the server IoC module
	public sealed class ZoneClientHandlerRegisterationAutofacModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			IEnumerable<Type> handlerTypes = LoadHandlerTypes().ToArray();

			//Debug.Log($"Found Handlers: {handlerTypes.Count()}");

			//Registers each type.
			foreach(Type t in handlerTypes)
			{
				builder.RegisterType(t)
					.AsSelf()
					.SingleInstance();

				//Debug.Log($"Registered Handler Type: {t.Name}");
			}
				

			foreach(Type t in handlerTypes)
			{
				Type concretePayloadType = t.GetTypeInfo()
					.ImplementedInterfaces
					.First(i => i.GetTypeInfo().IsGenericType && i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,>))
					.GetGenericArguments()
					.First();

				Type tryHandlerType = typeof(TrySemanticsBasedOnTypePeerMessageHandler<,,>)
					.MakeGenericType(typeof(GameServerPacketPayload), typeof(GameClientPacketPayload), concretePayloadType);

				builder.Register(context =>
					{
						object handler = context.Resolve(t);

						return Activator.CreateInstance(tryHandlerType, handler);
					})
					.As<IPeerMessageHandler<GameServerPacketPayload, GameClientPacketPayload>>()
					.As<IPeerMessageHandler<GameServerPacketPayload, GameClientPacketPayload, IPeerMessageContext<GameClientPacketPayload>>>()
					.SingleInstance();
			}
		}

		private IReadOnlyCollection<Type> LoadHandlerTypes()
		{
			return GetType().GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,>)) && !t.IsAbstract)
				.Where(t => t != typeof(ZoneClientDefaultRequestHandler))
				.ToArray();
		}
	}
}
