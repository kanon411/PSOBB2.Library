using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace Guardians
{
	[Table("guild_entry")]
	public class GuildEntryModel
	{
		/// <summary>
		/// Primary key of the guild entry.
		/// </summary>
		[Key]
		public int GuildId { get; private set; }

		//Registered unique index in builder
		/// <summary>
		/// The name of the guild.
		/// </summary>
		[MaxLength(32)] //length of a guild name in WoW. Seems like a good choice.
		public string GuildName { get; private set; }

		/// <summary>
		/// The character id foriegn key for the guild master.
		/// </summary>
		[ForeignKey(nameof(GuildMaster))]
		public int GuildMasterCharacterId { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel GuildMaster { get; private set; }

		/// <summary>
		/// The creation time of the friendship.
		/// </summary>
		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreationDate { get; private set; }

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private GuildEntryModel()
		{
			
		}
	}
}
