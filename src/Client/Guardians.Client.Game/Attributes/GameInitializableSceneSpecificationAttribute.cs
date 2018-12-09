﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Metadata attribute for marking <see cref="IGameInitializable"/>s and linking them to specific
	/// scenes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class GameInitializableSceneSpecificationAttribute : Attribute
	{
		/// <summary>
		/// Enumeration of scene types for a game initializable.
		/// </summary>
		public enum SceneType
		{
			TitleScreen = 0,

			ZoneGameScene = 1,
			
			CharacterSelection = 2,

			WorldDownloadingScreen = 3
		}

		/// <summary>
		/// The scene.
		/// </summary>
		public SceneType Scene { get; }

		/// <inheritdoc />
		public GameInitializableSceneSpecificationAttribute(SceneType scene)
		{
			if(!Enum.IsDefined(typeof(SceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(SceneType));
			Scene = scene;
		}
	}
}
