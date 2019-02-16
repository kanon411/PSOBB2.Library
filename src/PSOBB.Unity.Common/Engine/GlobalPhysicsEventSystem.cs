using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PSOBB
{
	public sealed class GlobalPhysicsEventSystem : IPhysicsTriggerEventSubscribable
	{
		private object SyncObj = new object();

		private Dictionary<PhysicsTriggerEventType, Action<object, PhysicsTriggerEventArgs>> PhysicsCallbackMap { get; }

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
	}
}
