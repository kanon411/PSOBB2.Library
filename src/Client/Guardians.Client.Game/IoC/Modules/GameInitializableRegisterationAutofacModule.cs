using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using Fasterflect;
using GladNet;
using UnityEngine;

namespace Guardians
{
	public sealed class GameInitializableRegisterationAutofacModule : Module
	{
		/// <summary>
		/// The scene to gather initializables for.
		/// </summary>
		[Tooltip("Should indicate the scene type to gather IGameInitializables for.")]
		[SerializeField]
		private GameInitializableSceneSpecificationAttribute.SceneType Scene = GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene;

		public GameInitializableRegisterationAutofacModule()
		{

		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			if(!Enum.IsDefined(typeof(GameInitializableSceneSpecificationAttribute.SceneType), Scene)) throw new InvalidEnumArgumentException(nameof(Scene), (int)Scene, typeof(GameInitializableSceneSpecificationAttribute.SceneType));

			foreach(var gameInit in GetType().Assembly.GetExportedTypes()
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
