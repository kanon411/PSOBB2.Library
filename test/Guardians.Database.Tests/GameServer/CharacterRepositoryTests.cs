using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public sealed class CharacterRepositoryTests
	{
		[Test]
		public static void ShouldAlwaysPass()
		{
			Assert.Pass();
		}

		[Test]
		public static void Test_MockedDatabaseContext_Can_Build()
		{
			//arange
			ServiceCollection collection = new ServiceCollection();

			IServiceProvider provider = collection.AddTestDatabaseContext<CharacterDatabaseContext>()
				.AddDefaultDataTestServices()
				.BuildServiceProvider();

			//act
			CharacterDatabaseContext result = provider.GetService<CharacterDatabaseContext>();

			//assert
			Assert.NotNull(result);
		}

		[Test]
		[TestCase("")]
		[TestCase("Andrew")]
		[TestCase("Lyle")]
		[TestCase("Shilo")]
		[TestCase("Sammy")]
		[TestCase("Skylar")]
		public static async Task Test_Contains_Name_False_On_Empty_Repository(string name)
		{
			//arrange
			ICharacterRepository repository = BuildEmptyRepository();

			//act
			bool result = await repository.ContainsAsync(name);

			//assert
			Assert.False(result, $"Expected empty repository with {nameof(ICharacterRepository.ContainsAsync)} to produce false on empy.");
		}

		[Test]
		[TestCase("Andrew")]
		[TestCase("Lyle")]
		[TestCase("Shilo")]
		[TestCase("Sammy")]
		[TestCase("Skylar")]
		public static async Task Test_Contains_Name_True_If_Model_Was_Added(string name)
		{
			//arrange
			ICharacterRepository repository = BuildEmptyRepository();

			//act
			await repository.TryCreateAsync(new CharacterEntryModel(1, name));

			bool result = await repository.ContainsAsync(name);

			//assert
			Assert.True(result, $"Expected true with {nameof(ICharacterRepository.ContainsAsync)}.");
		}

		public static ICharacterRepository BuildEmptyRepository()
		{
			ServiceCollection collection = new ServiceCollection();

			IServiceProvider provider = collection.AddTestDatabaseContext<CharacterDatabaseContext>()
				.AddDefaultDataTestServices()
				.AddTransient<ICharacterRepository, DatabaseBackedCharacterRepository>()
				.BuildServiceProvider();

			return provider.GetService<ICharacterRepository>();
		}
	}
}
