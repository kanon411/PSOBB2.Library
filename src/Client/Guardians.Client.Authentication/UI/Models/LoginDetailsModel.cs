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
			UserName = userName;
			Password = password;
		}

		protected LoginDetailsModel()
		{

		}
	}
}
