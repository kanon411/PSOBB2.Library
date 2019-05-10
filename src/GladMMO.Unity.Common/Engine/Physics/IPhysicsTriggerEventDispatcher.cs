using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GladMMO
{
	public interface IPhysicsTriggerEventDispatcher
	{
		void DispatchTriggerEnter(PhysicsTriggerEventType physicsType, GameObject objectTrigerRanOn, Collider colliderThatTriggered);

		void DispatchTriggerExit(PhysicsTriggerEventType physicsType, GameObject objectTrigerRanOn, Collider colliderThatTriggered);
	}
}
