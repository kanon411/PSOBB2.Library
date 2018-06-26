using System;

namespace Guardians
{
	public interface IReadonlyAuthTokenRepository
	{
		/// <summary>
		/// Retrieves the auth token.
		/// </summary>
		/// <returns>The authentication token.</returns>
		/// <exception cref="InvalidOperationException">Will throw if no auth token is registered.</exception>
		string Retrieve();
	}
}