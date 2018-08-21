using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class DefaultZoneServerApplicationBaseFactory : IFactoryCreatable<ApplicationBaseContainerPair, ZoneServerApplicationBaseCreationContext>
	{
		/// <inheritdoc />
		public ApplicationBaseContainerPair Create(ZoneServerApplicationBaseCreationContext context)
		{
			ContainerBuilder builder = new ContainerBuilder();

			//Old default: new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 5006)
			DefaultZoneServerDependencyRegistrar dependencyRegistrar = new DefaultZoneServerDependencyRegistrar(context.Logger, context.ServerAddress);

			dependencyRegistrar.RegisterServices(builder);

			IContainer build = builder.Build();

			return new ApplicationBaseContainerPair(build, build.Resolve<ZoneServerApplicationBase>());
		}
	}

	public sealed class ApplicationBaseContainerPair
	{
		public IContainer ServiceContainer { get; }

		public ZoneServerApplicationBase ApplicationBase { get; }

		/// <inheritdoc />
		public ApplicationBaseContainerPair([NotNull] IContainer serviceContainer, [NotNull] ZoneServerApplicationBase applicationBase)
		{
			ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
			ApplicationBase = applicationBase ?? throw new ArgumentNullException(nameof(applicationBase));
		}
	}
}
