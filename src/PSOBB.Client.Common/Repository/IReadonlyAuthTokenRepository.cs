using System;

namespace PSOBB
{
	public interface IReadonlyAuthTokenRepository
	{
		/// <summary>
		/// Retrieves the auth token.
		/// </summary>
		/// <returns>The authentication token.</returns>
		/// <exception cref="InvalidOperationException">Will throw if no auth token is registered.</exception>
		string Retrieve();

		/// <summary>
		/// Similar to <see cref="Retrieve"/> but includes the token type.
		/// Such as Bearer.
		/// </summary>
		/// <returns></returns>
		string RetrieveWithType();
	}
}