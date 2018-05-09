using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Guardians
{
	/// <summary>
	/// The model for the character database table.
	/// </summary>
	[Table("characters")]
	public class CharacterDatabaseModel
	{
		/// <summary>
		/// Primary key for the character.
		/// Must be unique.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CharacterId { get; private set; }

		/// <summary>
		/// Account ID associated with the character.
		/// </summary>
		[Required]
		public int AccountId { get; private set; }

		/// <summary>
		/// The name of the associated character.
		/// </summary>
		[Required]
		public string CharacterName { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreationDate { get; private set; }

		public CharacterDatabaseModel()
		{
			
		}
	}
}
