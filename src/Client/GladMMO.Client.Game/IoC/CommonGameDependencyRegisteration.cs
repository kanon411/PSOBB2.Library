using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using UnityEngine;

namespace GladMMO.Client
{
	public sealed class CommonGameDependencyRegisteration : AutofacBasedDependencyRegister<CommonGameDependencyModule>
	{
		[SerializeField]
		public GameSceneType SceneType;

		/// <inheritdoc />
		protected override CommonGameDependencyModule CreateModule()
		{
			UnityAsyncHelper.UnityUIAsyncContinuationBehaviour = this.gameObject.AddComponent<UnityUIAsyncContinuationBehaviour>();

			return new CommonGameDependencyModule(SceneType);
		}

		//TODO: Move this somewhere else
		internal class UnityUIAsyncContinuationBehaviour : MonoBehaviour
		{

		}
	}
}
