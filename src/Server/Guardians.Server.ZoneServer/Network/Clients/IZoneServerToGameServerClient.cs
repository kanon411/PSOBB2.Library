using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TypeSafe.Http.Net;

namespace Guardians
{
	//TODO: We shouldn't combine all the zoneserver query stuff in a single interface
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

		/// <summary>
		/// Queries the server for the location of the character with the provided <see cref="characterId"/>.
		/// </summary>
		/// <param name="characterId">The character id.</param>
		/// <returns>The location model of the character (potentially empty).</returns>
		[Get("/api/characters/location/{id}")]
		Task<ZoneServerCharacterLocationResponse> GetCharacterLocation([AliasAs("id")] int characterId);

		//TODO: Doc
		[Post("api/characters/location")]
		Task SaveCharacterLocation([JsonBody] ZoneServerCharacterLocationSaveRequest saveRequest);

		[Get("api/zoneserverdata/waypoint/{id}")]
		Task<ZoneServerWaypointQueryResponse> GetPathWaypoints([AliasAs("id")] int pathId);
	}
}
