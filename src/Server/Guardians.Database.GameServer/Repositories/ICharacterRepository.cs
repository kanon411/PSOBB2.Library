using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public interface ICharacterRepository : 
		IGenericRepositoryCrudable<int, CharacterEntryModel>
	{
		/// <summary>
		/// Checks if the repository contains a model with the specified charactername.
		/// </summary>
		/// <param name="characterName">The character name to check.</param>
		/// <returns>True if the name is taken.</returns>
		Task<bool> ContainsAsync(string characterName);

		/// <summary>
		/// Retrieves the name of the character by the provided
		/// <see cref="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>The name of the character. Throws if it doesn't exist.</returns>
		Task<string> RetrieveNameAsync(int key);
	}
}
