using System;
using System.Collections.Generic;
using System.Text;
using TypeSafe.Http.Net;

namespace Guardians
{
	/// <summary>
	/// Indicates that an auth token is required.
	/// Meaning that the Authorization header is dynamically provided.
	/// </summary>
	public sealed class AuthenticationTokenAttribute : DynamicHeaderAttribute
	{
		/// <inheritdoc />
		public AuthenticationTokenAttribute() 
			: base("Authorization")
		{

		}
	}
}
