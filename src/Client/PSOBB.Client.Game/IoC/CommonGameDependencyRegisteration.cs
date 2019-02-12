using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB.Client
{
	public sealed class CommonGameDependencyRegisteration : AutofacBasedDependencyRegister<CommonGameDependencyModule>
	{
		[SerializeField]
		public GameSceneType SceneType;

		/// <inheritdoc />
		protected override CommonGameDependencyModule CreateModule()
		{
			return new CommonGameDependencyModule(SceneType);
		}
	}
}
