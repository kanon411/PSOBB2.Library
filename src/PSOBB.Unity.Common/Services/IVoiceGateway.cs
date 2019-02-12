using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for voice management objects that allow enties to join.
	/// </summary>
	public interface IVoiceGateway
	{
		/// <summary>
		/// Joins the provided entity <see cref="entity"/>
		/// into the voice session.
		/// </summary>
		/// <param name="entity">The entity to join into the session.</param>
		void JoinVoiceSession(NetworkEntityGuid entity);

		/// <summary>
		/// Removes the provided entity <see cref="entity"/>
		/// from the voice session.
		/// </summary>
		/// <param name="entity">The entity to remove.</param>
		void LeaveVoiceSession(NetworkEntityGuid entity);
	}
}
