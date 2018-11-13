using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace Guardians
{
	public sealed class AuthenticationTokenAutofacModule : Module
	{
		public AuthenticationTokenAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			//This is used in many scenes to get and/or set the
			//the auth token for the session.
			builder.RegisterType<AuthenticationTokenRepository>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
