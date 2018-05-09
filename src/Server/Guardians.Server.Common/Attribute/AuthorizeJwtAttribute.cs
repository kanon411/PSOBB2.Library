using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Guardians
{
	/// <summary>
	/// Meta-data that marks an action/controller with an <see cref="AuthorizeAttribute"/>
	/// that indicates JWT should be used.
	/// </summary>
	public sealed class AuthorizeJwtAttribute : AuthorizeAttribute
	{
		/// <summary>
		/// Indicates that JWT should be used.
		/// </summary>
		public AuthorizeJwtAttribute()
		{
			AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
		}
	}
}
