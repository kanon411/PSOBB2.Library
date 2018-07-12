using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public abstract class InterestMonitorComponent : MonoBehaviour, IPhysicsTriggerCallbackable
	{
		[Inject]
		protected IGameObjectToEntityMappable ObjectToEntityMapper { get; }

		[Inject]
		protected ILog Logger { get; }

		[Inject]
		protected IInterestRadiusManager RadiusManager { get; }

		/// <inheritdoc />
		public abstract void OnTriggerEnter(Collider other);

		/// <inheritdoc />
		public abstract void OnTriggerExit(Collider other);
	}
}
