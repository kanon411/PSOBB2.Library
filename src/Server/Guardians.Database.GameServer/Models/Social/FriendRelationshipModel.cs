using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	[Table("character_friends")]
	public class FriendRelationshipModel
	{
		/// <summary>
		/// The primary key
		/// </summary>
		[Key]
		public int FriendshipId { get; private set; }

		/// <summary>
		/// The ID of the first character in the relationship.
		/// </summary>
		[ForeignKey(nameof(CharacterOne))]
		public int CharacterOneId { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel CharacterOne { get; private set; }

		/// <summary>
		/// The ID of the second character in the relationship.
		/// </summary>
		[ForeignKey(nameof(CharacterTwo))]
		public int CharacterTwoId { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel CharacterTwo { get; private set; }

		/// <summary>
		/// The creation time of the friendship.
		/// </summary>
		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreationDate { get; private set; }

		/// <inheritdoc />
		public FriendRelationshipModel(int characterOneId, int characterTwoId)
		{
			if(characterOneId <= 0) throw new ArgumentOutOfRangeException(nameof(characterOneId));
			if(characterTwoId <= 0) throw new ArgumentOutOfRangeException(nameof(characterTwoId));

			CharacterOneId = characterOneId;
			CharacterTwoId = characterTwoId;
		}

		private FriendRelationshipModel()
		{
			
		}
	}
}
