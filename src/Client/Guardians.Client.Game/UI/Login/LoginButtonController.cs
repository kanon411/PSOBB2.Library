using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.TitleScreen)]
	public sealed class LoginButtonController : IGameInitializable
	{
		/// <summary>
		/// The username text field.
		/// </summary>
		private LoginScreenUIElements UIElements { get; }

		/// <summary>
		/// The authentication service.
		/// </summary>
		private IAuthenticationService AuthService { get; }

		/// <summary>
		/// Logger.
		/// </summary>
		private ILog Logger { get; }

		/// <inheritdoc />
		public LoginButtonController(LoginScreenUIElements uiElements, IAuthenticationService authService, ILog logger)
		{
			UIElements = uiElements ?? throw new ArgumentNullException(nameof(uiElements));
			AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			//When login button is pressed we should temporarily disable the interactivity of the login button.
			UIElements.LoginButton.AddOnClickListener(() => UIElements.LoginButton.IsInteractable = false);

			//On start we have to actually register the login button callback
			//which is async, since it must send an auth request.
			UIElements.LoginButton.AddOnClickListenerAsync(OnLoginButtonClicked);
			return Task.CompletedTask;
		}

		public async Task OnLoginButtonClicked()
		{
			//TODO: Validate username and password
			JWTModel jwtModel = await AuthService.TryAuthenticate(BuildAuthRequestModel())
				.ConfigureAwait(false);

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Auth Response for User: {UIElements.UsernameText.Text} Result: {jwtModel.isTokenValid} OptionalError: {jwtModel.Error} OptionalErrorDescription: {jwtModel.ErrorDescription}");

			if(!jwtModel.isTokenValid)
			{
				await new UnityYieldAwaitable();
				UIElements.LoginButton.IsInteractable = true;
			}
		}

		private AuthenticationRequestModel BuildAuthRequestModel()
		{
			return new AuthenticationRequestModel(UIElements.UsernameText.Text, UIElements.PasswordText.Text);
		}
	}
}
