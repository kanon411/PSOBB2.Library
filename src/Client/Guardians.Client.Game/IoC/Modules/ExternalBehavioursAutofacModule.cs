using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Fasterflect;
using GladNet;

namespace Guardians
{
	public class ExternalBehavioursAutofacModule : Module
	{
		public ExternalBehavioursAutofacModule()
		{
			
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			foreach(var e in GetType().Assembly.GetExportedTypes()
				.Where(t => t.HasAttribute<ExternalBehaviourAttribute>()))
			{
				//All external behaviours are registered as themselves and multiple instances of them
				//should be created per request.
				builder.RegisterType(e)
					.AsSelf();
			}
		}
	}
}
