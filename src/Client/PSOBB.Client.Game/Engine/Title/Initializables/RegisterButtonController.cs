using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Refit;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.TitleScreen)]
	public sealed class RegisterButtonController : IGameInitializable
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
		public RegisterButtonController(
			[NotNull] LoginScreenUIElements uiElements, 
			[NotNull] IAuthenticationService authService, 
			[NotNull] ILog logger)
		{
			UIElements = uiElements ?? throw new ArgumentNullException(nameof(uiElements));
			AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			return Task.CompletedTask;
		}

		private async Task OnRegisterationButtonClicked()
		{
			string optionalResultMessage = null;
			try
			{
				//AuthService
				optionalResultMessage = await AuthService.TryRegister(UIElements.UsernameText.Text, UIElements.PasswordText.Text)
					.ConfigureAwait(false);
			}
			catch(ApiException e)
			{
				//Possible to get an ApiException here
				//Because 400 BadRequest is standard with OAuth
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Register; Account likely already exists: {optionalResultMessage}");

				//Just set this so later error handling works.
				if(optionalResultMessage == null)
					optionalResultMessage = "Error";
			}
			
			await new UnityYieldAwaitable();

			//We always renable on registeration
			UIElements.EnableInteractionOnAllButtons();

			//If it's not empty then we have a registeration error.
			if(!String.IsNullOrWhiteSpace(optionalResultMessage))
			{
				Logger.Error($"Failed to Register: {optionalResultMessage}");
				return;
			}

			//It's a valid registeration! Let's just log that
			Logger.Info($"Registered {UIElements.UsernameText.Text} sucessfully.");
		}
	}
}
