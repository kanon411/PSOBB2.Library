using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using GladNet;
using UnityEngine;
using Module = Autofac.Module;

namespace PSOBB
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
			foreach(Type handlerType in handlerTypes)
			{
				var handlerRegisterationBuilder = builder.RegisterType(handlerType)
					.AsSelf()
					.SingleInstance();

				//Now we need to register it as the additional specified types
				foreach(var additionalServiceTypeAttri in handlerType.GetCustomAttributes<AdditionalRegisterationAsAttribute>(true))
				{
					handlerRegisterationBuilder = handlerRegisterationBuilder
						.As(additionalServiceTypeAttri.ServiceType);
				}
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
