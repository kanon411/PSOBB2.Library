using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace GladMMO
{
	public sealed class DefaultMovementHandlerService : IMovementDataHandlerService
	{
		private IReadOnlyCollection<IMovementBlockHandler> MovementHandlers { get; }

		/// <inheritdoc />
		public DefaultMovementHandlerService([NotNull] IReadOnlyCollection<IMovementBlockHandler> movementHandlers)
		{
			MovementHandlers = movementHandlers ?? throw new ArgumentNullException(nameof(movementHandlers));
		}

		/// <inheritdoc />
		public bool CanHandle(IMovementData data)
		{
			//We can handle anything, most likely.
			return true;
		}

		/// <inheritdoc />
		public bool TryHandleMovement(NetworkEntityGuid entityGuid, IMovementData data)
		{
			foreach(var handler in MovementHandlers)
			{
				if(handler.CanHandle(data))
					return handler.TryHandleMovement(entityGuid, data);
			}

			return false;
		}
	}
}
