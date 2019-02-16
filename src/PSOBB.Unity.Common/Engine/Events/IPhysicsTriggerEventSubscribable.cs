using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public interface IPhysicsTriggerEventSubscribable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="physicsType">The type of physics.</param>
		/// <param name="physicsCallback">The callback to register.</param>
		void RegisterTriggerEventSubscription(PhysicsTriggerEventType physicsType, Action<object, PhysicsTriggerEventArgs> physicsCallback);
		
		//TODO: Add an unregister function.
	}

	public enum PhysicsTriggerEventType
	{
		Interest = 1,
	}

	public sealed class PhysicsTriggerEventArgs : EventArgs
	{
		/// <summary>
		/// The collider entering the trigger volume.
		/// </summary>
		public Collider TriggeringCollider { get; }

		/// <summary>
		/// The collider that the collider is enters.
		/// </summary>
		public Collider TriggeredCollider { get; }

		/// <inheritdoc />
		public PhysicsTriggerEventArgs([JetBrains.Annotations.NotNull] Collider triggeringCollider, [JetBrains.Annotations.NotNull] Collider triggeredCollider)
		{
			TriggeringCollider = triggeringCollider ?? throw new ArgumentNullException(nameof(triggeringCollider));
			TriggeredCollider = triggeredCollider ?? throw new ArgumentNullException(nameof(triggeredCollider));
		}
	}
}
