using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Enumeration of all response for a world download URL.
	/// </summary>
	public enum WorldDownloadURLResponseCode
	{
		/// <summary>
		/// Indicates the request was successful.
		/// </summary>
		Success = 1,

		/// <summary>
		/// Indicates that the world does not exist.
		/// </summary>
		NoWorld = 2,

		/// <summary>
		/// Indicates that the service for downloading worlds is unavailable.
		/// </summary>
		WorldDownloadServiceUnavailable = 3,

		/// <summary>
		/// Indicates that the world download is not authorize for the
		/// requesting user.
		/// </summary>
		AuthorizationFailed = 4
	}
}