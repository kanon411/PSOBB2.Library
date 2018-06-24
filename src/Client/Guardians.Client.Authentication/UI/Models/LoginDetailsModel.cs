using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Data model for authentication details.
	/// </summary>
	public sealed class LoginDetailsModel : IUserAuthenticationDetailsContainer
	{
		/// <inheritdoc />
		public string UserName { get; private set; }

		/// <inheritdoc />
		public string Password { get; private set; }

		/// <inheritdoc />
		public LoginDetailsModel(string userName, string password)
		{
			UserName = userName ?? throw new ArgumentNullException(nameof(userName));
			Password = password ?? throw new ArgumentNullException(nameof(password));
		}

		protected LoginDetailsModel()
		{

		}
	}
}
