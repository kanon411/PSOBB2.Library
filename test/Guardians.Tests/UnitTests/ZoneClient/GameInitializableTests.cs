using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Internal;
using Fasterflect;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Guardians
{
	[TestFixture]
	public sealed class GameInitializableTests
	{
		public static IReadOnlyCollection<Type> GameInitializables
			=> typeof(IGameInitializable).Assembly.GetExportedTypes()
				.Where(t => t.Implements(typeof(IGameInitializable)))
				.ToArrayTryAvoidCopy();

		[Test]
		[TestCaseSource(nameof(GameInitializables))]
		public void Test_All_GameInitablizes_Have_GameSceneAttribute(Type t)
		{
			//arrange

			//act
			bool hasAttribute = t.HasAttribute<GameInitializableSceneSpecificationAttribute>();

			//assert
			Assert.True(hasAttribute, $"Type: {t.Name} does not have {nameof(GameInitializableSceneSpecificationAttribute)}. All {nameof(IGameInitializable)} must have this attribute.");
		}
	}
}
