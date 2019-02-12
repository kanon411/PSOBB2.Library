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

namespace PSOBB
{
	public sealed class GameInitializableRegisterationAutofacModule : Module
	{
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		private GameSceneType Scene { get; }

		/// <summary>
		/// Default autofac ctor.
		/// </summary>
		public GameInitializableRegisterationAutofacModule()
		{
			//We shouldn't call this, I don't think.
		}

		/// <inheritdoc />
		public GameInitializableRegisterationAutofacModule(GameSceneType scene)
		{
			if(!Enum.IsDefined(typeof(GameSceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(GameSceneType));
			Scene = scene;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			if(!Enum.IsDefined(typeof(GameSceneType), Scene)) throw new InvalidEnumArgumentException(nameof(Scene), (int)Scene, typeof(GameSceneType));

			Load(builder, GetType().Assembly);
		}

		public void Load(ContainerBuilder builder, Assembly assemblyContainsInitializableTypes)
		{
			foreach(var gameInit in assemblyContainsInitializableTypes.GetExportedTypes()
				.Where(t => t.Implements(typeof(IGameInitializable)))
				.Where(t => t.Attributes<SceneTypeCreateAttribute>().Any(a => a.SceneType == Scene)))
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
