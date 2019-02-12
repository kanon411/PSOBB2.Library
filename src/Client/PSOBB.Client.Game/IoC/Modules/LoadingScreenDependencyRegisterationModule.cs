using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public sealed class LoadingScreenDependencyRegisterationModule : AutofacBasedDependencyRegister<LoadingScreenDefaultAutofacModule>
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		protected override LoadingScreenDefaultAutofacModule CreateModule()
		{
			//We need to provide the Service Disc URL to the auth module so we have to override
			return new LoadingScreenDefaultAutofacModule(ServiceDiscoveryUrl);
		}
	}
}
