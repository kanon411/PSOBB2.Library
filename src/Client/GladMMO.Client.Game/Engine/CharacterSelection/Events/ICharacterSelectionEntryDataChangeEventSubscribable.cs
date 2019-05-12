using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface ICharacterSelectionEntryDataChangeEventSubscribable
	{
		event EventHandler<CharacterSelectionEntryDataChangeEventArgs> OnCharacterSelectionEntryChanged;
	}

	public sealed class CharacterSelectionEntryDataChangeEventArgs : EventArgs
	{
		public ObjectGuid CharacterEntityGuid { get; }

		/// <inheritdoc />
		public CharacterSelectionEntryDataChangeEventArgs([NotNull] ObjectGuid characterEntityGuid)
		{
			CharacterEntityGuid = characterEntityGuid ?? throw new ArgumentNullException(nameof(characterEntityGuid));
		}
	}
}
