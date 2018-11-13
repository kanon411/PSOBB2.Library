using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using Common.Logging;
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
			//Set the sync context
			UnityExtended.InitializeSyncContext();

			builder.RegisterType<LoginScreenUIElements>()
				.AsSelf()
				.WithAttributeFiltering();

			builder.Register(context => LogLevel.All)
				.As<LogLevel>()
				.SingleInstance();

			builder.RegisterType<UnityLogger>()
				.As<ILog>()
				.SingleInstance();
		}
	}
}
