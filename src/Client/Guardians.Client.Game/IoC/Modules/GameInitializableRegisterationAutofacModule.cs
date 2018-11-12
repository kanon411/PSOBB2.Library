using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Features.AttributeFilters;
using Fasterflect;
using GladNet;

namespace Guardians
{
	public sealed class GameInitializableRegisterationAutofacModule : Module
	{
		public GameInitializableRegisterationAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			//New IPeerContext generic param now so we register as implemented interface
			foreach(var gameInit in GetType().Assembly.GetExportedTypes()
				.Where(t => t.Implements(typeof(IGameInitializable))))
			{
				builder.RegisterType(gameInit)
					.As<IGameInitializable>()
					.AsSelf()
					.SingleInstance()
					//TODO: We don't want to have to manually deal with this, we should create Attribute/Metadata to determine if this should be enabled.
					.WithAttributeFiltering();
			}
		}
	}
}
