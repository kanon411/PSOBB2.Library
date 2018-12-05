using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Guardians
{
	public sealed class AsyncEndpointContentServerService : BaseAsyncEndpointService<IContentServerServiceClient>, IContentServerServiceClient
	{
		/// <inheritdoc />
		public AsyncEndpointContentServerService(Task<string> futureEndpoint) 
			: base(futureEndpoint)
		{
		}

		/// <inheritdoc />
		public AsyncEndpointContentServerService(Task<string> futureEndpoint, RefitSettings settings) 
			: base(futureEndpoint, settings)
		{
		}

		/// <inheritdoc />
		public async Task<RequestedUrlResponseModel> GetNewWorldUploadUrl(string authToken)
		{
			return await (await GetService().ConfigureAwait(false)).GetNewWorldUploadUrl(authToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<WorldDownloadURLResponse> RequestWorldDownloadUrl(long worldId, string authToken)
		{
			return await (await GetService().ConfigureAwait(false)).RequestWorldDownloadUrl(worldId, authToken).ConfigureAwait(false);
		}
	}
}
