using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	[Table("claimed_sessions")]
	public class ClaimedSessionsModel
	{
		[Key]
		[Required]
		[ForeignKey(nameof(Session) + "," + nameof(CharacterEntry))]
		public int CharacterId { get; private set; }

		/// <summary>
		/// Navigation property to the linked session
		/// </summary>
		public virtual CharacterSessionModel Session { get; private set; }

		/// <summary>
		/// Navigation property to the linked session
		/// </summary>
		public virtual CharacterEntryModel CharacterEntry { get; private set; }

		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime SessionCreationDate { get; private set; }
	}
}
