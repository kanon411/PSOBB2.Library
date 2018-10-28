using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TypeSafe.Http.Net;

namespace Guardians
{
	//TODO: We need to do authorization headers for zoneserver stuff
	[Header("User-Agent", "ZoneServer")]
	public interface IZoneServerToGameServerClient
	{
		/// <summary>
		/// Requests the NPC data associated with the provided <see cref="mapId"/>.
		/// </summary>
		/// <param name="mapId">The ID of the map to load the NPC entries for.</param>
		/// <returns>HTTP response.</returns>
		[Header("Cache-Control", "max-age=5000")]
		[Get("/api/npcdata/map/{id}")]
		Task<ZoneServerNPCEntryCollectionResponse> GetNPCEntriesByMapId([AliasAs("id")] int mapId);
	}
}
