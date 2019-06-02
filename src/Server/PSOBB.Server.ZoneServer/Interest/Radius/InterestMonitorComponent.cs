﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public abstract class InterestMonitorComponent : MonoBehaviour, IPhysicsTriggerCallbackable
	{
		/// <inheritdoc />
		public abstract void OnTriggerEnter(Collider other);

		/// <inheritdoc />
		public abstract void OnTriggerExit(Collider other);
	}
}
