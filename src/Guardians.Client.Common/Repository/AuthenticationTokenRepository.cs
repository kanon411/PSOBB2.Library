using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class AuthenticationTokenRepository : IReadonlyAuthTokenRepository, IAuthTokenRepository
	{
		private static readonly object SyncObj = new object();

		//For ease-of-use between scenes in Unity3D we store this statically.
		//This is very bad, usually, but it's an issue between sharing data between Unity3D.
		/// <summary>
		/// The authentication token.
		/// </summary>
		private static string AuthToken { get; set; } = null;

		/// <inheritdoc />
		public string Retrieve()
		{
			lock(SyncObj)
			{
				if(AuthToken == null)
					throw new InvalidOperationException("Auth token it not initialized.");

				return AuthToken;
			}
		}

		/// <inheritdoc />
		public void Update([NotNull] string authToken)
		{
			if(string.IsNullOrEmpty(authToken)) throw new ArgumentException("Value cannot be null or empty.", nameof(authToken));

			lock(SyncObj)
			{
				AuthToken = authToken;
			}
		}
	}
}
