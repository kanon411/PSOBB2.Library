using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

		public void OnLoginButtonPressed()
		{
			//Load the model
			IUserAuthenticationDetailsContainer details = DetailsRepository.Retrieve();

			//Validate the object
			var validateResult = TryValidate(details);

			//TODO: Handle error case.
			if(!validateResult.IsValid)
				foreach(string s in validateResult
					.Where(pair => pair.Value.ValidationState != ModelValidationState.Valid)
					.Select(pair => $"{pair.Key}: {pair.Value}"))
				{
					Debug.LogError($"Model Validation Failed: {s}");
				}
				
			//Else we should try to auth.
			//TODO: Handle this properly.
			AuthClient.TryAuthenticateAsync(details)
				.UnityAsyncContinueWith(this, jwt =>
				{
					Debug.Log($"Auth Result: {jwt.isTokenValid} OptionalError: {jwt.Error}");
				});
		}

		public void UpdatePasswordValue(string password)
		{
			if(password == null) throw new ArgumentNullException(nameof(password));

			DetailsRepository.SetCurrent(new LoginDetailsModel(DetailsRepository.Retrieve().UserName, password));
		}

		public void UpdateUsernameValue(string username)
		{
			if(username == null) throw new ArgumentNullException(nameof(username));

			DetailsRepository.SetCurrent(new LoginDetailsModel(username, DetailsRepository.Retrieve().Password));
		}
	}
}
