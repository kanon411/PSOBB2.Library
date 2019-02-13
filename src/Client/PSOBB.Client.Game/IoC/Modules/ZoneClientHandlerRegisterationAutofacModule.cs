using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private GameSceneType SceneType { get; }

		/// <inheritdoc />
		public ZoneClientHandlerRegisterationAutofacModule(GameSceneType sceneType)
		{
			if(!Enum.IsDefined(typeof(GameSceneType), sceneType)) throw new InvalidEnumArgumentException(nameof(sceneType), (int)sceneType, typeof(GameSceneType));

			SceneType = sceneType;
		}

		private ZoneClientHandlerRegisterationAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			IEnumerable<Type> handlerTypes = LoadHandlerTypes().ToArray();

			StringBuilder handlerResultString = new StringBuilder(200);

			//Registers each type.
			foreach(Type handlerType in handlerTypes)
			{
				//TODO: Improve efficiency of all this reflection we are doing.
				IEnumerable<SceneTypeCreateAttribute> attributes = handlerType.GetCustomAttributes<SceneTypeCreateAttribute>(false);

				//We just skip now instead. For ease, maybe revert
				if(attributes == null || !attributes.Any())  //don't use base attributes
					continue;

				handlerResultString.AppendLine($"Register Handler: {handlerType.Name}");

				bool isForSceneType = DetermineIfHandlerIsForSceneType(handlerType, SceneType);

				//if it's not for the specified scene type, then skip.
				if(!isForSceneType)
					continue;

				var handlerRegisterationBuilder = builder.RegisterType(handlerType)
					.AsSelf()
					.As<IPeerMessageHandler<GameServerPacketPayload, GameClientPacketPayload>>()
					.As<IPeerMessageHandler<GameServerPacketPayload, GameClientPacketPayload, IPeerMessageContext<GameClientPacketPayload>>>();

				//Now we need to register it as the additional specified types
				foreach(var additionalServiceTypeAttri in handlerType.GetCustomAttributes<AdditionalRegisterationAsAttribute>(true))
				{
					handlerResultString.AppendLine($"\t As Also: Register Handler: {additionalServiceTypeAttri.ServiceType}");

					handlerRegisterationBuilder = handlerRegisterationBuilder
						.As(additionalServiceTypeAttri.ServiceType);
				}

				//Only ever want one handler, otherwise... things get werid with AdditionalRegisterationAsAttributes.
				handlerRegisterationBuilder = handlerRegisterationBuilder.SingleInstance();

				handlerResultString.AppendLine("\n\n");
			}

			Debug.Log(handlerResultString.ToString());
		}

		private IReadOnlyCollection<Type> LoadHandlerTypes()
		{
			return GetType().GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IPeerMessageHandler<,>)) && !t.IsAbstract)
				.Where(t => t != typeof(ZoneClientDefaultRequestHandler))
				.ToArray();
		}

		private static bool DetermineIfHandlerIsForSceneType(Type handlerType, GameSceneType sceneType)
		{
			//We don't want to get base attributes
			//devs may want to inherit from a handler and change some stuff. But not register it as a handler
			//for the same stuff obviously.
			foreach(SceneTypeCreateAttribute attris in handlerType.GetCustomAttributes<SceneTypeCreateAttribute>(false))
			{
				if(attris.SceneType == sceneType)
					return true;
			}

			return false;
		}
	}
}
