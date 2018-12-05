using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Guardians
{
	//TODO: We should seperate editor service from the general content server that the client reads.
	//From ProjectVindictive: https://github.com/HelloKitty/ProjectVindictive.Library/blob/master/src/ProjectVindictive.SDK.Editor.Build/Client/IUserContentManagementServiceClient.cs
	//TODO: Automate user-agent SDK version headers
	[Headers("User-Agent: SDK 0.0.1")]
	public interface IContentServerServiceClient
	{
		/// <summary>
		/// Attempts to get a new URL that can be used to upload the world.
		/// If successful the URl contained in the response will contain a valid upload
		/// URL which can be used to upload world content.
		/// </summary>
		/// <param name="authToken">The user authentication token.</param>
		/// <returns>A model representing the result of the world URL generation request.</returns>
		[Post("/api/World/create")]
		Task<RequestedUrlResponseModel> GetNewWorldUploadUrl([AuthenticationToken] string authToken);

		//TODO: Doc
		[Post("/api/World/{id}/downloadurl")]
		Task<WorldDownloadURLResponse> RequestWorldDownloadUrl([AliasAs("id")] long worldId, [AuthenticationToken] string authToken);
	}
}