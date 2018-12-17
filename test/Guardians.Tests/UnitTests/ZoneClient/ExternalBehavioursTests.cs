using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using Fasterflect;
using GladNet;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public class ExternalBehavioursTests
	{
		private static IEnumerable<Type> ExternalBehaviourTypes => typeof(ILocalPlayerDetails)
			.Assembly.GetExportedTypes()
			.Where(t => t.HasAttribute<ExternalBehaviourAttribute>() && !t.IsAbstract);

		[Test]
		[TestCaseSource(nameof(ExternalBehaviourTypes))]
		public void Test_Can_Resolve_ExternalBehaviour_Type(Type t)
		{
			//arrange
			ContainerBuilder builder = TestIoC.CreateDefaultContainer();

			IContainer resolver = builder.Build();

			object eb = null;

			//act
			try
			{
				eb = resolver.Resolve(t);
			}
			catch(DependencyResolutionException e)
			{
				//This makes it so the error is more readable. So we can see the exact dependency that is missing.
				DependencyResolutionException dependencyResolveException = e;

				while(dependencyResolveException.InnerException is DependencyResolutionException)
					dependencyResolveException = (DependencyResolutionException)dependencyResolveException.InnerException;

				Assert.Fail($"Failed: {dependencyResolveException.Message}\n\n{dependencyResolveException.StackTrace}");
			}

			Assert.NotNull(eb);
		}
	}
}
