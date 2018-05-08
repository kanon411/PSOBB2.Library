using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Consul.Net;
using HaloLive.Models.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Guardians
{
	public class Startup
	{
		//Changed in ASP Core 2.0
		public Startup(IConfiguration config)
		{
			if(config == null) throw new ArgumentNullException(nameof(config));

			GeneralConfiguration = config;
		}

		public IConfiguration GeneralConfiguration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<IISOptions>(options =>
			{
				options.ForwardClientCertificate = false;
				options.AutomaticAuthentication = false;
			});

			// Add framework services.
			services.AddMvc()
				.RegisterHealthCheckController();

			services.AddLogging();
			services.AddOptions();
			services.Configure<AuthenticationServerConfigurationModel>(GeneralConfiguration.GetSection("AuthConfig"));

			//We need to immediately resolve the authserver config options because we need them to regiter openiddict
			IOptions<AuthenticationServerConfigurationModel> authOptions = services.BuildServiceProvider()
				.GetService<IOptions<AuthenticationServerConfigurationModel>>();

			services.AddAuthentication();

			//Below is the OpenIddict registration
			//This is the recommended setup from the official Github: https://github.com/openiddict/openiddict-core
			services.AddIdentity<GuardiansApplicationUser, GuardiansApplicationRole>(options =>
				{
					//These disable the ridiculous requirements that the defauly password scheme has
					options.Password.RequireNonAlphanumeric = false;

					//For some reason I can't figure out how to get the JWT middleware to spit out sub claims
					//so we need to map the Identity to expect nameidentifier
					options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
					options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
					options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
				})
				.AddEntityFrameworkStores<GuardiansAuthenticationDbContext>()
				.AddDefaultTokenProviders();

			services.AddDbContext<GuardiansAuthenticationDbContext>(options =>
			{
				//TODO: Setup db options

				//On local builds we don't want to use config. We want to default to local
#if !DEBUG_LOCAL && !RELEASE_LOCAL
				options.UseMySql(authOptions.Value.AuthenticationDatabaseString);
#else
				options.UseMySql("Server=localhost;Database=guardians.auth;Uid=root;Pwd=test;");
#endif
				options.UseOpenIddict<int>();
			});

			services.AddOpenIddict<int>(options =>
			{
				// Register the Entity Framework stores.
				options.AddEntityFrameworkCoreStores<GuardiansAuthenticationDbContext>();

				//This will disable the https requirement if we're debugging or not in production/debug mode.
#if DEBUG || DEBUGBUILD
				options.DisableHttpsRequirement();
#endif
				// Register the ASP.NET Core MVC binder used by OpenIddict.
				// Note: if you don't call this method, you won't be able to
				// bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
				options.AddMvcBinders();

				//This controller endpoint/action was specified in the HaloLive documentation: https://github.com/HaloLive/Documentation
				options.EnableTokenEndpoint(authOptions.Value.AuthenticationControllerEndpoint); // Enable the token endpoint (required to use the password flow).
				options.AllowPasswordFlow(); // Allow client applications to use the grant_type=password flow.
				options.AllowRefreshTokenFlow();
				options.UseJsonWebTokens();

#warning Don't deploy this into production; we should use HTTPS. Even if it is behind IIS or HAProxy etc.
				options.DisableHttpsRequirement();

				try
				{
					//Loads the cert from the specified path
					options.AddSigningCertificate(X509Certificate2Loader.Create(Path.Combine(Directory.GetCurrentDirectory(), authOptions.Value.JwtSigningX509Certificate2Path)).Load());
				}
				catch(Exception e)
				{
					throw new InvalidOperationException($"Failed to load cert at Path: {authOptions.Value.JwtSigningX509Certificate2Path} with Root: {Directory.GetCurrentDirectory()}. Error: {e.Message} \n\n Stack: {e.StackTrace}", e);
				}
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
#warning Do not deploy exceptions page into production
			app.UseDeveloperExceptionPage();

			loggerFactory.RegisterGuardiansLogging(GeneralConfiguration);

			loggerFactory.AddDebug();

			app.UseAuthentication();

			app.UseMvcWithDefaultRoute();

			//TODO: Refactor into common service library so boilerplate registeration doesn't need to be repeated
			//TODO: Load Consul agent URI from file
			//TODO: Handle region tag loading from config
			//After the pipeline is configured we should then register this service with Consul.
			/*IConsulClient<IConsulAgentServiceHttpApiService> agentService = 
				new ConsulDotNetHttpClient<IConsulAgentServiceHttpApiService>(@"http://localhost:8500");

			//TODO: Handle logging better. We don't want to close just because of Consul
			//We will be orphaned though.
			try
			{
				Task.Factory.StartNew(async () =>
				{
					await Task.Delay(2000)
						.ConfigureAwait(false);

					//TODO: Validate this
					//The builder context actually has the listener URL that we need.
					var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
					string address = serverAddressesFeature.Addresses.First();

					Uri hostUri = new Uri(address);

					await agentService.Service.RegisterService(new AgentServiceRegisterationRequest()
						{
							//See: https://github.com/aspnet/Hosting/blob/a63932a492513cdeb4935661145084cad2ae5521/src/Microsoft.AspNetCore.Hosting.Abstractions/HostingAbstractionsWebHostBuilderExtensions.cs#L147
							//TODO: How should we decide public address?
							Address = $"{hostUri.Scheme}://{hostUri.Host}",
							Port = hostUri.Port,
							Id = Guid.NewGuid().ToString(),
							Name = "Authentication",

							//TODO: Handle locale from config; prod vs dev too
							Tags = new[] {"US", "Dev"}
						})
						.ConfigureAwait(false);
				});
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}*/
		}
	}
}
