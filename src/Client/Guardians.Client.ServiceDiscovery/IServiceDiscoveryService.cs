using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloLive.Models.NameResolution;
using TypeSafe.Http.Net;

namespace Guardians
{
	/// <summary>
	/// Contract for REST service that provides
	/// services discovery endpoints.
	/// </summary>
	[Header("User-Agent", "GuardiansClient")]
	public interface IServiceDiscoveryService
	{
		/// <summary>
		/// Attempts to discover a service with the provided <see cref="request"/>
		/// model.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>The result of the resolve request.</returns>
		[Post("/api/ServiceDiscovery")]
		Task<ResolveServiceEndpointResponseModel> DiscoverService([JsonBody] ResolveServiceEndpointRequestModel request);
	}
}
