using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;
using SceneJect.Common;

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

		public void OnLoginButtonPressed()
		{
			
		}

		public void UpdatePasswordValue(string password)
		{
			
		}

		public void UpdateUsernameValue(string username)
		{

		}
	}
}
