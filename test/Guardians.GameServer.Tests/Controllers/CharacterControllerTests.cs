using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public class CharacterControllerTests
	{
		[Test]
		public void Test_Can_Build_Mocked_Controller()
		{
			//arrange
			CharacterController controller = BuildCharacterController();

			//assert
			Assert.NotNull(controller);
		}

		[Test]
		public async Task Test_Get_Characters_Empty_Datastore_Produces_Empty_Character_Result()
		{
			//arrange
			IServiceProvider serviceProvider = BuildServiceProvider("Test", 1);
			CharacterController controller = serviceProvider.GetService<CharacterController>();

			//act
			CharacterListResponse response = await controller.GetCharacters();

			//assert
			Assert.NotNull(response);
			Assert.NotNull(response.CharacterIds);
			Assert.True(response.CharacterIds.Count == 0);
			Assert.True(response.ResultCode == CharacterListResponseCode.NoCharactersFoundError);
			Assert.False(response.isSuccessful);
		}

		[Test]
		public async Task Test_Get_Characters_Contains_Same_Size_As_Characters_Added([Range(1, 20)] int count)
		{
			//arrange
			IServiceProvider serviceProvider = BuildServiceProvider("Test", 1);
			CharacterController controller = serviceProvider.GetService<CharacterController>();

			List<string> names = await AddTestValuesToRepository(count, serviceProvider);

			//act
			CharacterListResponse response = await controller.GetCharacters();

			//assert
			Assert.AreEqual(count, response.CharacterIds.Count);
			Assert.True(response.ResultCode == CharacterListResponseCode.Success);
			Assert.True(response.isSuccessful);
		}

		//This test is incredibly important. We do not want to ever spit out characters from another account somehow
		[Test]
		public async Task Test_Get_Characters_Contains_No_Characters_Added_From_Other_Account([Range(1, 20)] int count)
		{
			//arrange
			IServiceProvider serviceProvider = BuildServiceProvider("Test", 1);
			CharacterController controller = serviceProvider.GetService<CharacterController>();

			List<string> names = await AddTestValuesToRepository(count, serviceProvider, 2);

			//act
			CharacterListResponse response = await controller.GetCharacters();

			//assert
			Assert.AreEqual(0, response.CharacterIds.Count, $"Expected NO characters to be provided. They're all under a different account. This is CRITICAL!");
			Assert.True(response.ResultCode == CharacterListResponseCode.NoCharactersFoundError);
			Assert.False(response.isSuccessful);
		}

		[Test]
		public async Task Test_Can_Get_Name_From_Character_Id([Range(1, 20)] int count)
		{
			//arrange
			IServiceProvider serviceProvider = BuildServiceProvider("Test", 1);
			CharacterController controller = serviceProvider.GetService<CharacterController>();

			List<string> names = await AddTestValuesToRepository(count, serviceProvider, 2);

			List<string> resultQueryNames = new List<string>(count);

			//assert
			for(int i = 1; i < count + 1; i++)
			{
				CharacterNameQueryResponse nameQueryResponse = GetActionResultObject<CharacterNameQueryResponse>(await controller.NameQuery(i));
				Assert.True(nameQueryResponse.isSuccessful);
				Assert.NotNull(nameQueryResponse.CharacterName);
				resultQueryNames.Add(nameQueryResponse.CharacterName);
			}

			//Check that each inserted name is available in the total query results
			foreach(string s in names)
				Assert.True(resultQueryNames.Contains(s));
		}

		public static T GetActionResultObject<T>(IActionResult result)
		{
			if(result == null) throw new ArgumentNullException(nameof(result));

			ObjectResult objectResult = (result as ObjectResult);

			if(objectResult?.Value == null) throw new InvalidOperationException($"Failed to get object Type: {typeof(T).Name} from IActionResult.");

			if(!typeof(T).IsAssignableFrom(objectResult.Value.GetType()))
				throw new InvalidOperationException($"Result objects is not of Type: {typeof(T).Name} was Type: {objectResult.Value.GetType().Name}");

			return (T)objectResult.Value;
		}

		private static async Task<List<string>> AddTestValuesToRepository(int count, IServiceProvider serviceProvider, int accountId = 1)
		{
			ICharacterRepository repository = serviceProvider.GetService<ICharacterRepository>();

			List<string> names = new List<string>(count);
			//Use the 1 key. Same above we add to the claims
			for(int i = 1; i < count + 1; i++)
			{
				string s = Guid.NewGuid().ToString();
				names.Add(s);

				await repository.TryCreateAsync(new CharacterEntryModel(accountId, s));
			}

			return names;
		}

		public static CharacterController BuildCharacterController(string userName, int accountId)
		{
			return BuildServiceProvider(userName, accountId).GetService<CharacterController>();
		}

		public static IServiceProvider BuildServiceProvider(string userName, int accountId)
		{
			Mock<IClaimsPrincipalReader> claimsReaderMock = new Mock<IClaimsPrincipalReader>();

			claimsReaderMock.Setup(c => c.GetUserName(It.IsAny<ClaimsPrincipal>()))
				.Returns(() => userName);

			claimsReaderMock.Setup(c => c.GetUserId(It.IsAny<ClaimsPrincipal>()))
				.Returns(accountId.ToString);

			Mock<ILogger<CharacterController>> loggingMock = new Mock<ILogger<CharacterController>>();

			IServiceProvider serviceProvider = new ServiceCollection()
				.AddTestDatabaseContext<CharacterDatabaseContext>()
				.AddDefaultDataTestServices()
				.AddSingleton<ICharacterRepository, DatabaseBackedCharacterRepository>()
				.AddTransient<CharacterController>()
				.AddTransient<IClaimsPrincipalReader>(provider => claimsReaderMock.Object)
				.AddTransient<ILogger<CharacterController>>(provider => loggingMock.Object)
				.BuildServiceProvider();

			return serviceProvider;
		}

		public static CharacterController BuildCharacterController()
		{
			return BuildCharacterController("Test", 1);
		}
	}
}
