using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Guardians
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				//.ConfigureKestrelHostWithCommandlinArgs(args)
				.UseKestrel()
				.UseUrls(@"http://0.0.0.0:7777")
				.UseIISIntegration()
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((context, builder) =>
				{
					//We now reigter this out here in ASP Core 2.0
					builder.AddJsonFile(@"Config/authserverconfig.json", false);
				})
				.UseApplicationInsights()
				.Build();
	}
}
