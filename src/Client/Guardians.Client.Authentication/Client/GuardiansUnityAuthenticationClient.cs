using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class GuardiansUnityAuthenticationClient
	{
		[Inject]
		private IAuthenticationService AuthService { get; }

		/// <inheritdoc />
		public GuardiansUnityAuthenticationClient(IAuthenticationService authService)
		{
			AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
		}

		public async Task<bool> TryAuthenticateAsync(IUserAuthenticationDetailsContainer detailsContainer)
		{
			if(detailsContainer == null) throw new ArgumentNullException(nameof(detailsContainer));

			if(String.IsNullOrWhiteSpace(detailsContainer.Password))
				ThrowInvalidAuthDetails("Password");

			if(String.IsNullOrWhiteSpace(detailsContainer.Password))
				ThrowInvalidAuthDetails("Username");

			//TODO: Store JWT if it was successful.
			JWTModel authModelResponse = await AuthService.TryAuthenticate(new AuthenticationRequestModel(detailsContainer.UserName, detailsContainer.Password));

			//TODO: Logging
			if(!authModelResponse.isTokenValid)
				return false;

			//TODO: Better flow for result so that we can do error handling and pops
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ThrowInvalidAuthDetails(string authFieldName)
		{
			if(string.IsNullOrWhiteSpace(authFieldName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(authFieldName));

			throw new InvalidOperationException($"Failed to auth. Authfield: {authFieldName} is not a valid state.");
		}
	}
}
