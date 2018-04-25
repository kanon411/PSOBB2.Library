using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Consul.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
			services.AddLogging();

			//We're using an inmemory store for now that we populate with the file stored data
			//It needs to be singleton because we're doing it in memory and reloading per request would be bad
			//services.AddDbContext<NamedEndpointDbContext>(options => options.UseInMemoryDatabase(), ServiceLifetime.Singleton);
			//services.AddSingleton<IRegionNamedEndpointStoreRepository, FilestoreBasedRegionNamedEndpointStoreRepository>();
			//services.AddTransient<IRegionbasedNameEndpointResolutionRepository, DatabaseContextBasedRegionBasedNameEndpointResolutionRepository>();

			//We're using consul now
			services.AddTransient<IRegionbasedNameEndpointResolutionRepository, ConsulRegionNamedEndpointStoreRepository>();

			//TODO: Do config
			services.AddTransient<IConsulClient<IConsulCatalogServiceHttpApiService>, ConsulDotNetHttpClient<IConsulCatalogServiceHttpApiService>>(p =>
			{
				//TODO: Add config loading for Consul
				return new ConsulDotNetHttpClient<IConsulCatalogServiceHttpApiService>(@"http://localhost:8500");
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseMvcWithDefaultRoute();
		}
	}
}
