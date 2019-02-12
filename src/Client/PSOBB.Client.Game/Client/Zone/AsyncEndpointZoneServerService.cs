using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Guardians
{
	public sealed class AsyncEndpointZoneServerService : BaseAsyncEndpointService<IZoneServerService>, IZoneServerService
	{
		/// <inheritdoc />
		public AsyncEndpointZoneServerService(Task<string> futureEndpoint) 
			: base(futureEndpoint)
		{

		}

		/// <inheritdoc />
		public AsyncEndpointZoneServerService(Task<string> futureEndpoint, RefitSettings settings) 
			: base(futureEndpoint, settings)
		{

		}

		/// <inheritdoc />
		public async Task<long> GetZoneWorld(int zoneId)
		{
			return await (await GetService().ConfigureAwait(false)).GetZoneWorld(zoneId).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<ResolveServiceEndpointResponse> GetServerEndpoint(int zoneId)
		{
			return await (await GetService().ConfigureAwait(false)).GetServerEndpoint(zoneId).ConfigureAwait(false);
		}
	}
}
