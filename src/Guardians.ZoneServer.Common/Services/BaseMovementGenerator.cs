using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Base for movement generators that control client and serverside movement simulation.
	/// </summary>
	/// <typeparam name="TDataInputType">The data input type.</typeparam>
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

		protected void Start(GameObject entity, long currentTime)
		{
			if(!hasStartFired)
				InternalStart(entity, currentTime);
			else
				throw new InvalidOperationException($"{nameof(Start)} should only ever be called once for a generator.");

			hasStartFired = true;
		}

		protected abstract void InternalStart(GameObject entity, long currentTime);

		/// <inheritdoc />
		public void Update(GameObject entity, float deltaTime)
		{
			if(!hasStartFired)
			{
				throw new InvalidOperationException($"Cannot start path before {nameof(Start)} is called.");
			}
			else 
				InternalUpdate(entity, deltaTime); //don't update if we called Start
		}

		//TODO: Doc
		/// <summary>
		/// Called on <see cref="Update"/>
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="deltaTime"></param>
		protected abstract void InternalUpdate(GameObject entity, float deltaTime);
	}
}
