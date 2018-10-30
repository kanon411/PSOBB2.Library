using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Guardians
{
	[JsonObject]
	public sealed class ZoneServerCharacterLocationSaveRequest
	{
		//TODO: Should we use network entity guid?
		/// <summary>
		/// The character ID to save the position of.
		/// </summary>
		[JsonProperty]
		public int CharacterId { get; }
		
		/// <summary>
		/// The position to save.
		/// </summary>
		[JsonConverter(typeof(Vector3Converter))]
		[JsonProperty]
		public Vector3 Position { get; private set; }

		/// <summary>
		/// The ID of the map the character is located in.
		/// </summary>
		[JsonProperty]
		public int MapId { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="characterId"></param>
		/// <param name="position"></param>
		/// <param name="mapId"></param>
		public ZoneServerCharacterLocationSaveRequest(int characterId, Vector3 position, int mapId)
		{
			CharacterId = characterId;
			Position = position;
			MapId = mapId;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected ZoneServerCharacterLocationSaveRequest()
		{
			
		}
	}
}
