using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Guardians
{
	public static class IWebBuilderExtensions
	{
		/// <summary>
		/// Configure the server host with the arguments encoded in the <see cref="args"/> mapped to
		/// an options instance of <see cref="DefaultWebHostingArgumentsModel"/>.
		/// </summary>
		/// <param name="builder">The web host builder.</param>
		/// <param name="args">The commandline args.</param>
		/// <returns>The provided <see cref="IWebHostBuilder"/>for fluent chaining.</returns>
		public static IWebHostBuilder ConfigureKestrelHostWithCommandlinArgs(this IWebHostBuilder builder, string[] args)
		{
			Parser.Default.ParseArguments<DefaultWebHostingArgumentsModel>(args)
				.WithParsed(model =>
				{
#if NETCOREAPP2_0
					//If https is enabled then a certifcate should be available for loading.
					builder.UseKestrel(options =>
					{

						int port = 5000;
						//Get the port
						if(model.isCustomUrlDefined)
						{
							int.TryParse(model.Url.Split(':').Last(), out port);
						}

						//Remov http
						string ip = model.Url.Replace("http://", "");
						ip = ip.Replace("https://", "");
						if(ip.Contains(':'))
							ip = ip.Split(':').First();

						options.Listen(new IPEndPoint(IPAddress.Parse(ip), port), listenOptions =>
						{
							if(model.isHttpsEnabled)
							{
								var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
								{
									//TODO: Do we need this in ASP Core 2.0?
									//ClientCertificateMode = ClientCertificateMode.AllowCertificate,

									//TODO: Mono doesn't support Tls1 or Tls2 and we have no way to config this. 
									//Ssl3 is mostly safe and supported by Mono which means it will work in Unity3D now.
									SslProtocols = System.Security.Authentication.SslProtocols.Tls
										| System.Security.Authentication.SslProtocols.Tls11
										| System.Security.Authentication.SslProtocols.Tls12,

									ServerCertificate = X509Certificate2Loader.Create(model.HttpsCertificateName).Load()
								};

								listenOptions.UseHttps(httpsConnectionAdapterOptions);
							}
						});

						//We still need this for the purposes for Features addresses
						//It won't conflict with the above
						if(model.isCustomUrlDefined)
						{
							builder.UseUrls(model.isHttpsEnabled
								? model.Url
									.ToLower()
									.Replace(@"http://", @"https://")
								: model.Url);
						}
						else
						{
							string prefix = model.isHttpsEnabled ? @"https://" : @"http://";
							builder.UseUrls($@"{prefix}localhost:5000");
						}
					});
#else
					if(model.isHttpsEnabled)
					{
						builder.UseKestrel(options =>
						{
							options.UseHttps(new HttpsConnectionFilterOptions()
							{
								//TODO: Mono doesn't support Tls1 or Tls2 and we have no way to config this. 
								//Ssl3 is mostly safe and supported by Mono which means it will work in Unity3D now.
								SslProtocols = System.Security.Authentication.SslProtocols.Tls
									| System.Security.Authentication.SslProtocols.Tls11
									| System.Security.Authentication.SslProtocols.Tls12,

								//Load the cert with the cert loader
								ServerCertificate = X509Certificate2Loader.Create(model.HttpsCertificateName).Load()
							});
						});
					}
					else
						builder.UseKestrel();

					//TODO: Should we allow both https and non-https traffic?
					if(model.isCustomUrlDefined)
					{
						builder.UseUrls(model.isHttpsEnabled
							? model.Url
								.ToLower()
								.Replace(@"http://", @"https://")
							: model.Url);
					}
					else
					{
						string prefix = model.isHttpsEnabled ? @"https://" : @"http://";
						builder.UseUrls($@"{prefix}localhost:5000");
					}
				
#endif
				});

			return builder;
		}
	}
}
