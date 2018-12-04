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
		Success = 0,

		/// <summary>
		/// Indicates that the world does not exist.
		/// </summary>
		NoWorld = 1,

		/// <summary>
		/// Indicates that the service for downloading worlds is unavailable.
		/// </summary>
		WorldDownloadServiceUnavailable = 2,

		AuthorizationFailed = 3,
	}
}