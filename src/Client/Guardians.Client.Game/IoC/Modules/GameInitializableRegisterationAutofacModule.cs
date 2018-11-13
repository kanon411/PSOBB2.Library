using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using Fasterflect;
using GladNet;
using UnityEngine;
using Module = Autofac.Module;

namespace Guardians
{
	public sealed class GameInitializableRegisterationAutofacModule : Module
	{
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		private GameInitializableSceneSpecificationAttribute.SceneType Scene { get; }

		/// <summary>
		/// Default autofac ctor.
		/// </summary>
		public GameInitializableRegisterationAutofacModule()
		{
			//We shouldn't call this, I don't think.
		}

		/// <inheritdoc />
		public GameInitializableRegisterationAutofacModule(GameInitializableSceneSpecificationAttribute.SceneType scene)
		{
			if(!Enum.IsDefined(typeof(GameInitializableSceneSpecificationAttribute.SceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(GameInitializableSceneSpecificationAttribute.SceneType));
			Scene = scene;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			if(!Enum.IsDefined(typeof(GameInitializableSceneSpecificationAttribute.SceneType), Scene)) throw new InvalidEnumArgumentException(nameof(Scene), (int)Scene, typeof(GameInitializableSceneSpecificationAttribute.SceneType));

			Load(builder, GetType().Assembly);
		}

		public void Load(ContainerBuilder builder, Assembly assemblyContainsInitializableTypes)
		{
			foreach(var gameInit in assemblyContainsInitializableTypes.GetExportedTypes()
				.Where(t => t.Implements(typeof(IGameInitializable)))
				.Where(t => t.Attributes<GameInitializableSceneSpecificationAttribute>().Any(a => a.Scene == Scene)))
			{
				builder.RegisterType(gameInit)
					.As<IGameInitializable>()
					.AsSelf()
					.SingleInstance()
					//TODO: We don't want to have to manually deal with this, we should create Attribute/Metadata to determine if this should be enabled.
					.WithAttributeFiltering();
			}
		}
	}
}
