using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using GladNet;

namespace Guardians
{
	public sealed class LoginScreenDependenciesAutofacModule : Module
	{
		public LoginScreenDependenciesAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<LoginScreenUIElements>()
				.AsSelf()
				.WithAttributeFiltering();
		}
	}
}
