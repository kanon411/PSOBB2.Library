using System;
using System.ComponentModel.DataAnnotations;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

namespace Guardians
{
	/// <summary>
	/// Data model for authentication details.
	/// </summary>
	public sealed class TextFieldBackedLoginDetailsModel : IUserAuthenticationDetailsContainer
	{
		private InputField UsernameField { get; }

		private InputField PasswordField { get; }

		/// <inheritdoc />
		[Required]
		[StringLength(64, ErrorMessage = nameof(UserName) + "must be the proper length.", MinimumLength = 1)]
		public string UserName => UsernameField.text;

		/// <inheritdoc />
		[Required]
		[StringLength(64, ErrorMessage = nameof(Password) + "must be the proper length.", MinimumLength = 1)]
		public string Password => PasswordField.text;

		/// <inheritdoc />
		public TextFieldBackedLoginDetailsModel(InputField usernameField, InputField passwordField)
		{
			UsernameField = usernameField ?? throw new ArgumentNullException(nameof(usernameField));
			PasswordField = passwordField ?? throw new ArgumentNullException(nameof(passwordField));
		}

		protected TextFieldBackedLoginDetailsModel()
		{
			
		}
	}
}