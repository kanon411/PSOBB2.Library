using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace GladMMO
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

		}
	}
}
