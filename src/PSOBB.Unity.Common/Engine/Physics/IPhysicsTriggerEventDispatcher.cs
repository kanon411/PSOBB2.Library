using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public interface IPhysicsTriggerEventDispatcher
	{
		void DispatchTriggerEnter(PhysicsTriggerEventType physicsType, Collider colliderThatRanTrigger, Collider colliderThatTriggered);

		void DispatchTriggerExit(PhysicsTriggerEventType physicsType, Collider colliderThatRanTrigger, Collider colliderThatTriggered);
	}
}
