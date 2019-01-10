using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Guardians.Social.ManualTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			IAuthenticationService authService = Refit.RestService.For<IAuthenticationService>("http://192.168.0.3:5001");

			Console.Write($"Username: ");
			string username = Console.ReadLine();

			Console.Write($"Password: ");
			string password = Console.ReadLine();

			Console.Write($"CharacterId: ");
			string characterId = Console.ReadLine();

			string token = (await authService.TryAuthenticate(new AuthenticationRequestModel(username, password))
				.ConfigureAwait(false)).AccessToken;

			HubConnection connection = new HubConnectionBuilder()
				.WithUrl("http://192.168.0.3:5008/realtime/textchat", options =>
				{
					options.Headers.Add(SocialNetworkConstants.CharacterIdRequestHeaderName, characterId);
					options.AccessTokenProvider = () => Task.FromResult(token);
				})
				.Build();

			await connection.StartAsync()
				.ConfigureAwait(false);

			connection.On("Test", new Type[1] {typeof(string)}, objects =>
			{
				string response = objects[0] as string;

				Console.WriteLine($"Response: {response}");

				return Task.CompletedTask;
			});

			while(true)
			{
				string input = Console.ReadLine();

				await connection.InvokeAsync("Test", input);
			}
		}
	}
}
