using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Data model for authentication details.
	/// </summary>
	public sealed class LoginDetailsModel : IUserAuthenticationDetailsContainer
	{
		/// <inheritdoc />
		[Required]
		[StringLength(64, ErrorMessage = nameof(UserName) + "must be the proper length.", MinimumLength = 1)]
		public string UserName { get; private set; }

		/// <inheritdoc />
		[Required]
		[StringLength(64, ErrorMessage = nameof(Password) + "must be the proper length.", MinimumLength = 1)]
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
