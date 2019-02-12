using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// Contract for types that implement movement handling.
	/// </summary>
	public interface IMovementBlockHandler
	{
		/// <summary>
		/// True if the <see cref="data"/> can be handled by this handler.
		/// </summary>
		/// <param name="data">The movement data.</param>
		/// <returns>True if the data can be handled by this handler.</returns>
		bool CanHandle(IMovementData data);

		/// <summary>
		/// Attempts to handle the provided <see cref="data"/>.
		/// Will fail if <see cref="CanHandle"/> with provided <see cref="data"/>
		/// is false.
		/// </summary>
		/// <param name="entityGuid">The entity guid associated with the movement.</param>
		/// <param name="data">The movement data.</param>
		/// <returns>True if it was handled.</returns>
		bool TryHandleMovement(NetworkEntityGuid entityGuid, IMovementData data);
	}
}
