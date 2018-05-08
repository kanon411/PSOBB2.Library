﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Guardians
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddLogging();

#if !DEBUG_LOCAL && !RELEASE_LOCAL
			//TODO: Support database/consul/register gameservers
			//Probably wanna support healthchecks at some point too
			services.AddSingleton<IGameServersStoreRepository, DefaultDevelopmentGameServersRepository>();
#else
			services.AddSingleton<IGameServersStoreRepository, LocalDevelopmentGameServersRepository>();
#endif
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
#warning Do not deploy exceptions page into production
			app.UseDeveloperExceptionPage();

			loggerFactory.RegisterGuardiansLogging(Configuration);
			loggerFactory.AddDebug();

			app.UseMvc();
		}
	}
}