using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public sealed class InterestRadiusGatewayEntry : InterestMonitorComponent
	{
		/// <inheritdoc />
		public override void OnTriggerEnter(Collider other)
		{
			GlobalPhysicsEventSystem.Instance.DispatchTriggerEnter(PhysicsTriggerEventType.InterestEnter, gameObject, other);
		}

		/// <inheritdoc />
		public override void OnTriggerExit(Collider other)
		{

		}
	}
}
