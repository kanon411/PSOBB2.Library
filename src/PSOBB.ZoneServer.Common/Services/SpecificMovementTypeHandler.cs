using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	/// <summary>
	/// Handler Type for specific movement types.
	/// </summary>
	/// <typeparam name="TSpecificMovementType"></typeparam>
	public abstract class SpecificMovementTypeHandler<TSpecificMovementType> : IMovementBlockHandler
		where TSpecificMovementType : class, IMovementData
	{
		/// <inheritdoc />
		public bool CanHandle(IMovementData data)
		{
			return data is TSpecificMovementType;
		}

		/// <inheritdoc />
		public bool TryHandleMovement(NetworkEntityGuid entityGuid, IMovementData data)
		{
			bool result = CanHandle(data);

			if(result)
				HandleMovement(entityGuid, data as TSpecificMovementType);

			return result;
		}

		protected abstract void HandleMovement(NetworkEntityGuid entityGuid, TSpecificMovementType data);
	}
}
