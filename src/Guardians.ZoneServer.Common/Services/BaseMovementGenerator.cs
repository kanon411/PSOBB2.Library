using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public abstract class BaseMovementGenerator<TDataInputType> : IMovementGenerator<GameObject>
		where TDataInputType : class, IMovementData
	{
		/// <summary>
		/// The movement data used by this generator.
		/// </summary>
		protected TDataInputType MovementData { get; }

		protected bool hasStartFired { get; private set; }

		/// <inheritdoc />
		protected BaseMovementGenerator([NotNull] TDataInputType movementData)
		{
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
		}

		protected abstract void Start(GameObject entity, long currentTime);

		/// <inheritdoc />
		public void Update(GameObject entity, long currentTime)
		{
			if(!hasStartFired)
			{
				Start(entity, currentTime);
				hasStartFired = true;
			}
			else 
				InternalUpdate(entity, currentTime); //don't update if we called Start
		}

		/// <summary>
		/// Called on <see cref="Update"/>
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="currentTime"></param>
		protected abstract void InternalUpdate(GameObject entity, long currentTime);
	}
}
