using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using Nito.AsyncEx;
using Refit;
using UnityEngine.SceneManagement;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.TitleScreen)]
	public sealed class TitleScreenTryAuthenticateOnLoginButtonClickedEventListener : BaseSingleEventListenerInitializable<ILoginButtonClickedEventSubscribable>
	{
		/// <summary>
		/// The authentication service.
		/// </summary>
		private IAuthenticationService AuthService { get; }

		/// <summary>
		/// Logger.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The username text field.
		/// </summary>
		public IUIText UsernameText { get; }

		/// <summary>
		/// The password text field.
		/// </summary>
		public IUIText PasswordText { get; }

		/// <inheritdoc />
		public TitleScreenTryAuthenticateOnLoginButtonClickedEventListener(
			ILoginButtonClickedEventSubscribable subscriptionService, 
			IAuthenticationService authService, 
			ILog logger,
			[NotNull] [KeyFilter(UnityUIRegisterationKey.UsernameTextBox)] IUIText usernameText,
			[NotNull] [KeyFilter(UnityUIRegisterationKey.PasswordTextBox)] IUIText passwordText)
			: base(subscriptionService)
		{
			AuthService = authService;
			Logger = logger;
			UsernameText = usernameText;
			PasswordText = passwordText;
		}

		private AuthenticationRequestModel BuildAuthRequestModel()
		{
			return new AuthenticationRequestModel(UsernameText.Text, PasswordText.Text);
		}

		//TODO: Simplified async event firing/handling
		/// <inheritdoc />
		protected override void OnEventFired(object source, EventArgs args)
		{
			//We should not do async OnEventFired because we will get silent failures.
			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				JWTModel jwtModel = null;

				//TODO: Validate username and password
				//We can't do error code supression with refit anymore, so we have to do this crap.
				try
				{
					jwtModel = await AuthService.TryAuthenticate(BuildAuthRequestModel())
						.ConfigureAwait(false);
				}
				catch(ApiException e)
				{
					jwtModel = e.GetContentAs<JWTModel>();
				}

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Auth Response for User: {UsernameText.Text} Result: {jwtModel.isTokenValid} OptionalError: {jwtModel.Error} OptionalErrorDescription: {jwtModel.ErrorDescription}");
			});
		}
	}
}
