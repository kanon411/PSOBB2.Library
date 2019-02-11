using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using Refit;
using UnityEngine.SceneManagement;

namespace Guardians
{
	[SceneTypeCreate(GameSceneType.TitleScreen)]
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

		private IAuthTokenRepository AuthenticationTokenRepository { get; }

		private ISceneManager SceneService { get; }

		/// <inheritdoc />
		public LoginButtonController(
			LoginScreenUIElements uiElements, 
			IAuthenticationService authService, 
			ILog logger,
			IAuthTokenRepository authenticationTokenRepository,
			ISceneManager sceneService)
		{
			UIElements = uiElements ?? throw new ArgumentNullException(nameof(uiElements));
			AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			AuthenticationTokenRepository = authenticationTokenRepository ?? throw new ArgumentNullException(nameof(authenticationTokenRepository));
			SceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			//When login button is pressed we should temporarily disable the interactivity of the login button.
			UIElements.LoginButton.AddOnClickListener(() => UIElements.DisableInteractionOnAllButtons());

			//On start we have to actually register the login button callback
			//which is async, since it must send an auth request.
			UIElements.LoginButton.AddOnClickListenerAsync(OnLoginButtonClicked);
			return Task.CompletedTask;
		}

		public async Task OnLoginButtonClicked()
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
				Logger.Debug($"Auth Response for User: {UIElements.UsernameText.Text} Result: {jwtModel.isTokenValid} OptionalError: {jwtModel.Error} OptionalErrorDescription: {jwtModel.ErrorDescription}");

			await new UnityYieldAwaitable();

			if(!jwtModel.isTokenValid)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Authenticate: {jwtModel.ErrorDescription}");

				UIElements.EnableInteractionOnAllButtons();
			}
			else
			{
				//If we're successful then we'll just set the auth token for future consumers for this session
				AuthenticationTokenRepository.Update(jwtModel.AccessToken);

				//TODO: We should load to the next level.
				SceneService.LoadLevel((int)GameSceneType.CharacterSelection, LoadSceneMode.Single);
			}
		}

		private AuthenticationRequestModel BuildAuthRequestModel()
		{
			return new AuthenticationRequestModel(UIElements.UsernameText.Text, UIElements.PasswordText.Text);
		}
	}
}
