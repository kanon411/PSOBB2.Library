using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public sealed class GlobalPhysicsEventSystem : IPhysicsTriggerEventSubscribable, IPhysicsTriggerEventDispatcher
	{
		private object SyncObj = new object();

		private Dictionary<PhysicsTriggerEventType, Action<object, PhysicsTriggerEventArgs>> PhysicsCallbackMap { get; }

		/// <summary>
		/// The global physics callback system.
		/// </summary>
		public static GlobalPhysicsEventSystem Instance { get; } = new GlobalPhysicsEventSystem();

		public GlobalPhysicsEventSystem()
		{
			PhysicsCallbackMap = new Dictionary<PhysicsTriggerEventType, Action<object, PhysicsTriggerEventArgs>>();
		}

		/// <inheritdoc />
		public void RegisterTriggerEventSubscription(PhysicsTriggerEventType physicsType, [JetBrains.Annotations.NotNull] Action<object, PhysicsTriggerEventArgs> physicsCallback)
		{
			if(physicsCallback == null) throw new ArgumentNullException(nameof(physicsCallback));
			if(!Enum.IsDefined(typeof(PhysicsTriggerEventType), physicsType)) throw new InvalidEnumArgumentException(nameof(physicsType), (int)physicsType, typeof(PhysicsTriggerEventType));

			lock(SyncObj)
			{
				if(PhysicsCallbackMap.ContainsKey(physicsType))
				{
					PhysicsCallbackMap[physicsType] += physicsCallback;
				}
				else
					PhysicsCallbackMap.Add(physicsType, physicsCallback);
			}
		}

		/// <inheritdoc />
		public void DispatchPhysicsEvent(PhysicsTriggerEventType physicsType, Collider colliderThatRanTrigger, Collider colliderThatTriggered)
		{
			if(PhysicsCallbackMap.ContainsKey(physicsType))
			{
				Action<object, PhysicsTriggerEventArgs> callback = null;
				lock(SyncObj)
				{
					if(PhysicsCallbackMap.ContainsKey(physicsType))
						callback = PhysicsCallbackMap[physicsType];
				}

				callback?.Invoke(this, new PhysicsTriggerEventArgs(colliderThatRanTrigger, colliderThatTriggered));
			}
		}
	}
}
