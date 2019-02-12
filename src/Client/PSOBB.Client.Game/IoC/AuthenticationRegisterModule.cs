using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Simplified creatable type for <see cref="AuthenticationDependencyAutofacModule"/>
	/// </summary>
	public sealed class AuthenticationRegisterModule : AutofacBasedDependencyRegister<AuthenticationDependencyAutofacModule>
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		protected override AuthenticationDependencyAutofacModule CreateModule()
		{
			//We need to provide the Service Disc URL to the auth module so we have to override
			return new AuthenticationDependencyAutofacModule(ServiceDiscoveryUrl);
		}
	}
}
