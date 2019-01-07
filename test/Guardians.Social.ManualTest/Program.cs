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
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl("http://192.168.0.3:5008/Test")
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
