using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Database table model for friend requests.
	/// </summary>
	[Table("character_friendrequests")]
	public class CharacterFriendshipRequestModel
	{
		/// <summary>
		/// The primary key
		/// </summary>
		[Key]
		public int FriendshipRequestId { get; private set; }

		/// <summary>
		/// The ID of the character requesting the friend request.
		/// </summary>
		[ForeignKey(nameof(RequestingCharacter))]
		public int RequestingCharacterId { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel RequestingCharacter { get; private set; }

		/// <summary>
		/// The ID of the target for the friend request.
		/// </summary>
		[ForeignKey(nameof(TargetRequestCharacter))]
		public int TargetRequestCharacterId { get; private set; }

		/// <summary>
		/// Navigation property to the character table.
		/// </summary>
		public virtual CharacterEntryModel TargetRequestCharacter { get; private set; }

		/// <summary>
		/// The creation time of the friendship.
		/// </summary>
		[Required]
		[Column(TypeName = "TIMESTAMP(6)")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreationDate { get; private set; }

		/// <inheritdoc />
		public CharacterFriendshipRequestModel(int requestingCharacterId, int targetRequestCharacterId)
		{
			if(requestingCharacterId <= 0) throw new ArgumentOutOfRangeException(nameof(requestingCharacterId));
			if(targetRequestCharacterId <= 0) throw new ArgumentOutOfRangeException(nameof(targetRequestCharacterId));

			RequestingCharacterId = requestingCharacterId;
			TargetRequestCharacterId = targetRequestCharacterId;
		}

	}
}
