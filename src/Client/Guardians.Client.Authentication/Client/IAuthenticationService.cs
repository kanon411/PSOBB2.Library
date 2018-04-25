using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TypeSafe.Http.Net;

namespace Guardians
{
	/// <summary>
	/// Proxy interface for Authentication Server RPCs.
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Authenticate request method. Sends the request model as a URLEncoded body.
		/// See the documentation for information about the endpoint.
		/// https://github.com/HaloLive/Documentation
		/// </summary>
		/// <param name="request">The request model.</param>
		/// <returns>The authentication result.</returns>
		[SupressResponseErrorCodes((int)HttpStatusCode.BadRequest)] //OAuth spec returns 400 BadRequest on failed auth
		[Post("/api/auth")]
		Task<JWTModel> TryAuthenticate([UrlEncodedBody] AuthenticationRequestModel request);
	}
}
