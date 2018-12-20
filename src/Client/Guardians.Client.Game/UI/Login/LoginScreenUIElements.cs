using System;
using System.Collections.Generic;
using System.Text;
using Autofac.Features.AttributeFilters;

namespace Guardians
{
	public sealed class LoginScreenUIElements
	{
		/// <summary>
		/// The username text field.
		/// </summary>
		public IUIText UsernameText { get; }

		/// <summary>
		/// The password text field.
		/// </summary>
		public IUIText PasswordText { get; }

		/// <summary>
		/// The login button.
		/// </summary>
		public IUIButton LoginButton { get; }

		/// <inheritdoc />
		public LoginScreenUIElements(
			[KeyFilter(UnityUIRegisterationKey.UsernameTextBox)] IUIText usernameText,
			[KeyFilter(UnityUIRegisterationKey.PasswordTextBox)] IUIText passwordText,
			[KeyFilter(UnityUIRegisterationKey.Login)] IUIButton loginButton)
		{
			UsernameText = usernameText ?? throw new ArgumentNullException(nameof(usernameText));
			PasswordText = passwordText ?? throw new ArgumentNullException(nameof(passwordText));
			LoginButton = loginButton ?? throw new ArgumentNullException(nameof(loginButton));
		}
		public void DisableInteractionOnAllButtons()
		{
			LoginButton.IsInteractable = false;
		}

		public void EnableInteractionOnAllButtons()
		{
			LoginButton.IsInteractable = true;
		}
	}
}
