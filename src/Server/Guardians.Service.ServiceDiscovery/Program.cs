using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HaloLive.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Guardians
{
	public sealed class Program
	{
		/// <summary>
		/// Starts a new hosted web application with the following optional parameters
		/// HostedUrl...
		/// </summary>
		/// <param name="args">Optional parameters.</param>
		public static void Main(string[] args)
		{
			IWebHost host = new WebHostBuilder()
				.ConfigureKestrelHostWithCommandlinArgs(args)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.UseApplicationInsights()
				.Build();

			host.Run();
		}
	}
}
