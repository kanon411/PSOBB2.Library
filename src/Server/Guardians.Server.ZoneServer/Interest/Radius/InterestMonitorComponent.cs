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
		protected IReadonlyGameObjectToEntityMappable ObjectToEntityMapper { get; private set; }

		[Inject]
		protected ILog Logger { get; private set; }

		[Inject]
		protected IInterestRadiusManager RadiusManager { get; private set; }

		/// <inheritdoc />
		public abstract void OnTriggerEnter(Collider other);

		/// <inheritdoc />
		public abstract void OnTriggerExit(Collider other);
	}
}
