using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TypeSafe.Http.Net;

namespace Guardians
{
    public sealed class TypeSafeServiceDiscoveryServiceClient : IServiceDiscoveryService
    {
		public IServiceDiscoveryService Service { get; }

		/// <inheritdoc />
		public TypeSafeServiceDiscoveryServiceClient(string baseUrl)
		{
			if(string.IsNullOrEmpty(baseUrl)) throw new ArgumentException("Value cannot be null or empty.", nameof(baseUrl));

			Service = TypeSafeHttpBuilder<IServiceDiscoveryService>
				.Create()
				.RegisterDefaultSerializers()
				.RegisterJsonNetSerializer()
				.RegisterDotNetHttpClient(baseUrl)
				.Build();
		}

		/// <inheritdoc />
		public Task<ResolveServiceEndpointResponse> DiscoverService(ResolveServiceEndpointRequest request)
		{
			return Service.DiscoverService(request);
		}
	}
}
