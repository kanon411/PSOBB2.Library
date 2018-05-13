using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public abstract class GenericCrubRepositoryDefaultTests<TDbContextType, TRepositoryType, TKeyType, TModelType>
		where TDbContextType : DbContext
		where TRepositoryType : class, IGenericRepositoryCrudable<TKeyType, TModelType>
	{
		[Test]
		[TestCase(5)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(100)]
		[TestCase(int.MaxValue)]
		public async Task Test_Contains_Id_False_On_Empty_Repository(int key)
		{
			//arrange
			IGenericRepositoryCrudable<TKeyType, TModelType> repository = BuildEmptyRepository();

			TKeyType keyValue = TryBuildKey(key);

			//act
			bool result = await repository.ContainsAsync(keyValue);

			//assert
			Assert.False(result, $"Expected empty repository with {nameof(ICharacterRepository.ContainsAsync)} to produce false on empy.");
		}

		[Test]
		public async Task Test_Can_Add_New_Model_To_Repository()
		{
			//arrange
			IGenericRepositoryCrudable<TKeyType, TModelType> repository = BuildEmptyRepository();

			TModelType model = BuildRandomModel();

			//act
			bool addResult = await repository.TryCreateAsync(model);

			//assert
			Assert.True(addResult, $"Could not add Model: {model.ToString()} to repository for some reason.");
		}

		[Test]
		public async Task Test_Adding_Duplicate_Model_Throws_Exception_With_Exeption_Mentioning_Key()
		{
			//arrange
			IGenericRepositoryCrudable<TKeyType, TModelType> repository = BuildEmptyRepository();

			TModelType model = BuildRandomModel();

			//act
			bool addResult = await repository.TryCreateAsync(model);

			//assert
			Assert.ThrowsAsync<ArgumentException>(async () => await repository.TryCreateAsync(model));

			//Check message value
			try
			{
				await repository.TryCreateAsync(model);
			}
			catch(Exception e)
			{
				Assert.True(e.Message.ToLower().Contains("key"), $"Exception methods should contain the word key somewhere in their message.");
				Assert.True(e.Message.ToLower().Contains(ProduceKeyFromModel(model).ToString().ToLower()), $"Exceptions contain the key value in their message.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <exception cref="InconclusiveException">Throws if the key cannot be constructed.</exception>
		/// <returns></returns>
		private static TKeyType TryBuildKey(int key)
		{
			//TODO: Is there a better way to do this?
			TKeyType keyValue = default(TKeyType);
			try
			{
				keyValue = GenericMath.Convert<int, TKeyType>(key);
			}
			catch(Exception)
			{
				Assert.Inconclusive($"Unable to convert Key: {key} to Type: {typeof(TKeyType).Name} so skipping.");
			}

			return keyValue;
		}

		/// <summary>
		/// Should build a random model for use for the testing class.
		/// </summary>
		/// <returns></returns>
		public abstract TModelType BuildRandomModel();

		public abstract TKeyType ProduceKeyFromModel(TModelType model);

		public static IGenericRepositoryCrudable<TKeyType, TModelType> BuildEmptyRepository()
		{
			ServiceCollection collection = new ServiceCollection();

			IServiceProvider provider = collection
				.AddTestDatabaseContext<TDbContextType>()
				.AddDefaultDataTestServices()
				.AddTransient<IGenericRepositoryCrudable<TKeyType, TModelType>, TRepositoryType>()
				.BuildServiceProvider();

			return provider.GetService<IGenericRepositoryCrudable<TKeyType, TModelType>>();
		}
	}
}
