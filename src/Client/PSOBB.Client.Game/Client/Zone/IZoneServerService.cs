﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace PSOBB
{
	/// <summary>
	/// Proxy interface for zone server data service requests.
	/// </summary>
	[Headers("User-Agent: GuardiansClient")]
	public interface IZoneServerService
	{
		//TODO: Create response model instead, incase the zoneserver doesn't exist.
		/// <summary>
		/// Queries for the world id of the provided zone server
		/// with ID: <see cref="zoneId"/>
		/// </summary>
		/// <param name="zoneId">The zone id.</param>
		/// <returns>The id of the zoneserver. Throws if it doesn't exist.</returns>
		[Get("/api/zoneserver/{id}/worldid")]
		[Headers("Cache-Control: max-age=300")]
		Task<long> GetZoneWorld([AliasAs("id")] int zoneId);

		//TODO: We should add authorization to this
		[Get("/api/zoneserver/{id}/endpoint")]
		//[ResponseCache(Duration = 300)] //TODO: Cache
		Task<ResolveServiceEndpointResponse> GetServerEndpoint([AliasAs("id")] int zoneId);
	}
}
