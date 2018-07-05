using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Enumeration of all game payload operation codes.
	/// </summary>
	public enum GamePayloadOperationCode : int
	{
		/// <summary>
		/// For the session claim request from the client.
		/// </summary>
		ClientSessionClaimRequest = 1,
	}
}
