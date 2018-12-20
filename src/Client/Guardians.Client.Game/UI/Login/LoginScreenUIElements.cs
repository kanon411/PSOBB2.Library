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

		[Obsolete("We should not deploy this to prod.")]
		public IUIButton DevRegisterationButton { get; }

		/// <inheritdoc />
		public LoginScreenUIElements(
			[KeyFilter(UnityUIRegisterationKey.UsernameTextBox)] IUIText usernameText,
			[KeyFilter(UnityUIRegisterationKey.PasswordTextBox)] IUIText passwordText,
			[KeyFilter(UnityUIRegisterationKey.Login)] IUIButton loginButton,
			[NotNull] [KeyFilter(UnityUIRegisterationKey.Registeration)] IUIButton devRegisterationButton)
		{
			UsernameText = usernameText ?? throw new ArgumentNullException(nameof(usernameText));
			PasswordText = passwordText ?? throw new ArgumentNullException(nameof(passwordText));
			LoginButton = loginButton ?? throw new ArgumentNullException(nameof(loginButton));

			ProjectVersionStage.AssertInternalTesting();
			DevRegisterationButton = devRegisterationButton ?? throw new ArgumentNullException(nameof(devRegisterationButton));
		}

		public void DisableInteractionOnAllButtons()
		{
			LoginButton.IsInteractable = false;
			DevRegisterationButton.IsInteractable = false;
		}

		public void EnableInteractionOnAllButtons()
		{
			LoginButton.IsInteractable = true;
			DevRegisterationButton.IsInteractable = true;
		}
	}
}
