using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public interface IPhysicsTriggerEventDispatcher
	{
		void DispatchPhysicsEvent(PhysicsTriggerEventType physicsType, Collider colliderThatRanTrigger, Collider colliderThatTriggered);
	}
}
