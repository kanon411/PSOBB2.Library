using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;
using SceneJect.Common;
using UnityEngine;
using Unitysync.Async;

namespace Guardians
{
	[Injectee]
	public sealed class LoginFormController : UIController<LoginFormView>
	{
		/// <summary>
		/// Authentication client for the auth.
		/// </summary>
		[Inject]
		private IAuthenticationClient AuthClient { get; }

		[Inject]
		private IAuthDetailsRepository DetailsRepository { get; }

		[Inject]
		private IValidator<IUserAuthenticationDetailsContainer> AuthDetailsValidator { get; }

		public void OnLoginButtonPressed()
		{
			//Load the model
			IUserAuthenticationDetailsContainer details = DetailsRepository.Retrieve();

			//Validate the object
			var validateResult = AuthDetailsValidator.Validate(details);

			//TODO: Handle error case.
			if(!validateResult.IsValid)
			{
				if(Logger.IsErrorEnabled)
					foreach(string s in validateResult.Errors.Select(e => e.ErrorMessage))
					{
						Logger.Error(s);
					}
				return;
			}
			
			View.SetLoginButtonState(false);

			//Else we should try to auth.
			//TODO: Handle this properly.
			AuthClient.TryAuthenticateAsync(details)
				.UnityAsyncContinueWith(this, jwt =>
				{
					if(Logger.IsDebugEnabled)
						Logger.Debug($"Auth Result: {jwt.isTokenValid} OptionalError: {jwt.Error}");

					View.SetLoginButtonState(true);
				});
		}

		public void UpdatePasswordValue(string password)
		{
			if(password == null) throw new ArgumentNullException(nameof(password));

			DetailsRepository.SetCurrent(new LoginDetailsModel(DetailsRepository.Retrieve().UserName, password));
			CheckModelState();
		}

		private void CheckModelState()
		{
			View.SetLoginButtonState(AuthDetailsValidator.Validate(DetailsRepository.Retrieve()).IsValid);
		}

		public void UpdateUsernameValue(string username)
		{
			if(username == null) throw new ArgumentNullException(nameof(username));

			DetailsRepository.SetCurrent(new LoginDetailsModel(username, DetailsRepository.Retrieve().Password));
			CheckModelState();
		}
	}
}
