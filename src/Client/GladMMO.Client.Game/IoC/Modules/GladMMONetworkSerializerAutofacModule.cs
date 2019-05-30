using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace GladMMO
{
	public sealed class GladMMONetworkSerializerAutofacModule : Module
	{
		public GladMMONetworkSerializerAutofacModule()
		{

		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			/*builder.RegisterType<SerializerService>()
				.AsSelf()
				.As<ISerializerService>()
				.OnActivated(args =>
				{
					args.Instance.Compile();
				})
				.SingleInstance();

			builder.RegisterType<FreecraftCoreGladNetSerializerAdapter>()
				.AsSelf()
				.As<INetworkSerializationService>()
				.SingleInstance();*/
		}
	}
}