﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	[Table("character_locations")]
	public class CharacterLocationModel
	{
		/// <summary>
		/// The ID of the character this entry is related to.
		/// </summary>
		[Key]
		[ForeignKey(nameof(Character))]
		public int CharacterId { get; private set; }

		/// <summary>
		/// Indicates the zone type.
		/// (If this is transient we should check session table to reconnect to it maybe)
		/// </summary>
		[Required]
		public GameZoneType ZoneType { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel Character { get; private set; }

		/// <summary>
		/// The X position of the character.
		/// </summary>
		[Required]
		public float XPosition { get; private set; }

		/// <summary>
		/// The Y position of the character.
		/// </summary>
		[Required]
		public float YPosition { get; private set; }

		/// <summary>
		/// The Z position of the character.
		/// </summary>
		[Required]
		public float ZPosition { get; private set; }

		/// <summary>
		/// Indicates the last time the location was updated.
		/// </summary>
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime LastUpdated { get; private set; }

		/// <inheritdoc />
		public CharacterLocationModel(int characterId, GameZoneType zoneType, float xPosition, float yPosition, float zPosition)
		{
			if(characterId < 0) throw new ArgumentOutOfRangeException(nameof(characterId));
			if(!Enum.IsDefined(typeof(GameZoneType), zoneType)) throw new InvalidEnumArgumentException(nameof(zoneType), (int)zoneType, typeof(GameZoneType));

			CharacterId = characterId;
			ZoneType = zoneType;
			XPosition = xPosition;
			YPosition = yPosition;
			ZPosition = zPosition;
		}

		public CharacterLocationModel()
		{
			
		}
	}
}