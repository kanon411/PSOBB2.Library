using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using UnityEngine;

namespace PSOBB
{
	public sealed class AuthenticationTokenDependencyRegister : AutofacBasedDependencyRegister<AuthenticationTokenAutofacModule>
	{
		private class DemoAuthenticationModule : AuthenticationTokenAutofacModule
		{
			private string DemoToken { get; }

			/// <inheritdoc />
			public DemoAuthenticationModule(string demoToken)
			{
				DemoToken = demoToken ?? throw new ArgumentNullException(nameof(demoToken));
			}

			/// <inheritdoc />
			protected override void RegisterAuthenticationTokenRepository(ContainerBuilder builder)
			{
				builder.RegisterInstance(new AuthenticationTokenRepository(DemoToken))
					.AsImplementedInterfaces()
					.SingleInstance();
			}
		}

		[SerializeField]
		private bool EnableDemoToken;

		[SerializeField]
		private string DemoToken;

		/// <inheritdoc />
		protected override AuthenticationTokenAutofacModule CreateModule()
		{
			if(!EnableDemoToken)
				return base.CreateModule();

			//Otherwise, we are going to need a demo value
			return new DemoAuthenticationModule(DemoToken);
		}
	}
}
