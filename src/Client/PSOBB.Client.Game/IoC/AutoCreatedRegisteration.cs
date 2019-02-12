using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using Fasterflect;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	//TODO: Convert to AutoFac Module
	public class AutoCreatedRegisteration : NonBehaviourDependency
	{
		private static Type[] EngineTypes = new Type[] { typeof(IGameTickable), typeof(IGameInitializable) };

		//TODO: When we have specific floors or special scenes that don't fit type we may want to supply zone id or additional metadata.
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		[SerializeField]
		private GameSceneType SceneType;

		void Awake()
		{
			if(!Enum.IsDefined(typeof(GameSceneType), SceneType))
				throw new InvalidOperationException($"Invalid {nameof(GameSceneType)}: {(byte)SceneType}");
		}

		/// <inheritdoc />
		public override void Register(ContainerBuilder builder)
		{
			foreach(var creatable in GetType().Assembly.GetTypes()
				.Where(t => EngineTypes.Any(et => t.Implements(et)))
				.Where(t => t.Attributes<SceneTypeCreateAttribute>().Any(a => a.SceneType == SceneType))
				.Where(t => OnFilterCreationType(t)))
			{
				//TODO: DO we need register self?
				var registrationBuilder = builder.RegisterType(creatable)
					//.AsSelf()
					.SingleInstance()
					//TODO: We don't want to have to manually deal with this, we should create Attribute/Metadata to determine if this should be enabled.
					.WithAttributeFiltering();

				//We should also iterate all RegisterationAs attributes and register
				//the types under those too
				foreach(var regAttri in creatable.GetCustomAttributes<AdditionalRegisterationAsAttribute>(false))
				{
					registrationBuilder = registrationBuilder.As(regAttri.ServiceType);
				}

				foreach(Type engineType in EngineTypes)
					if(creatable.Implements(engineType))
						registrationBuilder = registrationBuilder.As(engineType);
			}
		}

		protected virtual bool OnFilterCreationType(Type t)
		{
			return true;
		}
	}
}
