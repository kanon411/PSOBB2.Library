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
				.WithUrl("http://192.168.0.3:5008/realtime/textchat", options =>
				{
					options.Headers.Add(SocialNetworkConstants.CharacterIdRequestHeaderName, "4");
					options.AccessTokenProvider = () => Task.FromResult("eyJhbGciOiJSUzI1NiIsImtpZCI6IjYyN0YyQUFDMTZERTlENjNDMkY3NDQyQzk1OUFBNjEyQjIyOTlENDciLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiIxIiwibmFtZSI6IkFkbWluIiwidG9rZW5fdXNhZ2UiOiJhY2Nlc3NfdG9rZW4iLCJqdGkiOiJlOGE1NDg2YS00NTAxLTRkOTYtYWM1ZC04Njg5ZmE3MTFlN2IiLCJhdWQiOiJhdXRoLXNlcnZlciIsIm5iZiI6MTU0MzYxOTQ5NCwiZXhwIjoxNTQzNjIzMDk0LCJpYXQiOjE1NDM2MTk0OTQsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMS8ifQ.tQy40fQvWKjPBtAwJ8J59f4FZj31POTDGsM2ZYiTfbtZgGyR299RRr-Z7GbCgwMorFOurcOcIrHwKW44wgYbuhoWuyoIXByZRvayLmGA9Lsj1y0eC-_twIOhUKTx-qzQjpdx-ZZHaqv5Akuz6KGYTHrXCjn2YDz-oiJwLHSMTt-J7-ILI29fPbTmPyLABUL0oEDD1TOdoDSRz_COE2HoD6LXoMmrgtxJmxOH4PfxG6Xgs_gC27Qgfiuo3uiVGkj8ELkCZ5fxQE1gA0xSWdl84iZZaOxlb6U6aecHhjZ7vaUbVAdCta0hr5dBgXUc85p9PcZMRRL0Q6zDOHJWTViDXUihUcE2OxG-_qkBnBj6Gw4tmUi4H_I5dKhT-1M3Ws1T3CxGGVUJNI0Gv7iAR42Ahr0vMEP9gfW_mMwkz4Xd5dmcTRinSdWEe2MM7u1oAagmqNRRtpxxV9QoKuOuegxzI3nD-7-B8Wqg_jKOwpIPKJYDCP3TQ9UnCE3ss6OfpqiFeLQm0gN9Ow_oHEDVC-r0gkkipkxYOdwhEF0VN3Nq661jBy_ji8s1OnE0G3gFGahJoU-ECwy0L2jzIad7AOGG0mQu1FmvqJhEdhlS9TWIkJdMzoebnG4RPghKoEFd2v0ptPRthksFdInPmJqQ7-202Fh3yvlzmDZLfy_ILpiz_1s");
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
