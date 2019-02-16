using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	public sealed class CommonInstanceServerDependencyRegister : NonBehaviourDependency
	{
		[SerializeField]
		public GameSceneType SceneType;

		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			register.RegisterModule(new EngineInterfaceRegisterationModule(SceneType, GetType().Assembly));
		}
	}
}
