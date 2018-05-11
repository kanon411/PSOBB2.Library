using System;
using System.Collections.Generic;
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

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime SessionCreationDate { get; private set; }

		//TODO: Add Zone navigation property

		public CharacterSessionModel()
		{
			
		}
	}
}
