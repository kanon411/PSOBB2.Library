﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PSOBB
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrelGuardiansConfig(args)
				.ConfigureServices(services => services.AddAutofac()) //this enables AutoFac configuration support
				//.UseKestrel()
				.UseIISIntegration()
				.UseStartup<Startup>()
				//TODO: remove this logging when we finally deploy properly
				.UseSetting("detailedErrors", "true")
				.CaptureStartupErrors(true)
				.UseApplicationInsights()
				.Build();
	}
}