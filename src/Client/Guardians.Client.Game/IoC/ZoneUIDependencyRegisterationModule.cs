using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace Guardians
{
	public sealed class ZoneUIDependencyRegisterationModule : NonBehaviourDependency
	{
		/// <inheritdoc />
		public override void Register(ContainerBuilder register)
		{
			foreach(var go in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				foreach(var registerable in go.GetComponentsInChildren<SerializedMonoBehaviour>()
					.Select(m => m as IUIAdapterRegisterable)
					.Where(m => m != null))
				{
					//Registers the adapter with the specified Key and Service Type.
					register.RegisterInstance(registerable)
						.SingleInstance()
						.Keyed(registerable.RegisterationKey, registerable.UISerivdeType);
				}
			}
		}
	}
}
