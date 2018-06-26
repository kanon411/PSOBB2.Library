using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Simple data access repository contract
	/// for the type <see cref="IUserAuthenticationDetailsContainer"/>.
	/// </summary>
	public interface IAuthDetailsRepository
	{
		/// <summary>
		/// Retrieves the current <see cref="IUserAuthenticationDetailsContainer"/>.
		/// </summary>
		/// <returns>The current auth details.</returns>
		IUserAuthenticationDetailsContainer Retrieve();

		/// <summary>
		/// Sets a new current <see cref="IUserAuthenticationDetailsContainer"/>.
		/// </summary>
		/// <param name="details">The details to set as the current model.</param>
		void SetCurrent(IUserAuthenticationDetailsContainer details);
	}
}
