using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;

namespace Guardians
{
	public sealed class DefaultZoneServerApplicationBaseFactory : IFactoryCreatable<ZoneServerApplicationBase, ZoneServerApplicationBaseCreationContext>
	{
		/// <inheritdoc />
		public ZoneServerApplicationBase Create(ZoneServerApplicationBaseCreationContext context)
		{
			ContainerBuilder builder = new ContainerBuilder();

			//Old default: new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 5006)
			DefaultZoneServerDependencyRegistrar dependencyRegistrar = new DefaultZoneServerDependencyRegistrar(context.Logger, context.ServerAddress);

			dependencyRegistrar.RegisterServices(builder);

			IContainer build = builder.Build();

			return build.Resolve<ZoneServerApplicationBase>();
		}
	}
}
