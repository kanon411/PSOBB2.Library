using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace PSOBB
{
	public class AuthenticationTokenAutofacModule : Module
	{
		public AuthenticationTokenAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			RegisterAuthenticationTokenRepository(builder);
		}

		protected virtual void RegisterAuthenticationTokenRepository(ContainerBuilder builder)
		{
			//This is used in many scenes to get and/or set the
			//the auth token for the session.
			builder.RegisterType<AuthenticationTokenRepository>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
