using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Simplified creatable type for <see cref="GameInitializableRegisterationAutofacModule"/>
	/// </summary>
	public sealed class GameInitializablesRegisterModule : AutofacBasedDependencyRegister<GameInitializableRegisterationAutofacModule>
	{
		/// <summary>
		/// The scene to gather initializables for.
		/// </summary>
		[Tooltip("Should indicate the scene type to gather IGameInitializables for.")]
		[SerializeField]
		private GameSceneType Scene = GameSceneType.ZoneGameScene;

		/// <inheritdoc />
		protected override GameInitializableRegisterationAutofacModule CreateModule()
		{
			return new GameInitializableRegisterationAutofacModule(Scene);
		}
	}
}
