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
		public NetworkEntityGuid CharacterEntityGuid { get; }

		/// <inheritdoc />
		public CharacterSelectionEntryDataChangeEventArgs([NotNull] NetworkEntityGuid characterEntityGuid)
		{
			CharacterEntityGuid = characterEntityGuid ?? throw new ArgumentNullException(nameof(characterEntityGuid));
		}
	}
}
