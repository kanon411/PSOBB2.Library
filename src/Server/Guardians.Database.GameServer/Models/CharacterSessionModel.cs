using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	[Table("character_sessions")]
	public class CharacterSessionModel
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SessionId { get; private set; }

		//This should be manually registered as an alternate key
		[Required]
		[ForeignKey(nameof(CharacterEntry))]
		public int CharacterId { get; private set; }

		public CharacterEntryModel CharacterEntry { get; private set; }

		//This should be manually registered as an alternate key
		[Required]
		public int AccountId { get; private set; }

		//This should be manually registered as an alternate key
		[Required]
		[ForeignKey(nameof(ZoneEntry))]
		public int ZoneId { get; private set; }

		//Navigation property
		public virtual ZoneInstanceEntryModel ZoneEntry { get; private set; }

		//Initially this will be false. Meaning there will be a short window of time
		//where users may request to claim a session multiple times. Meaning there could be
		//for example 5 clients may be claiming this session, be redirected to where it is it should be claimed
		//but the design should be that only one session is ever accepted. Only one can cause True to be set.
		//All others must be rejected.
		/// <summary>
		/// Indicates if a session is active. It means the character is not claimed.
		/// Meaning it should not exist in any zone instance.
		/// </summary>
		[Required]
		[DefaultValue(false)]
		public bool IsSessionActive { get; private set; }

		//Fluent API sets up default time
		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime SessionCreationDate { get; private set; }

		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime SessionLastUpdateDate { get; private set; }

		/// <inheritdoc />
		public CharacterSessionModel(int characterId, int zoneId, bool isSessionActive = false)
		{
			if(characterId < 0) throw new ArgumentOutOfRangeException(nameof(characterId));
			if(zoneId < 0) throw new ArgumentOutOfRangeException(nameof(zoneId));

			CharacterId = characterId;
			ZoneId = zoneId;
			IsSessionActive = isSessionActive;
		}

		public CharacterSessionModel()
		{
			
		}
	}
}
