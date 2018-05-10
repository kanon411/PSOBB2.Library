using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public interface ICharacterRepository : 
		IGenericRepositoryCrudable<int, CharacterDatabaseModel>
	{
		/// <summary>
		/// Checks if the repository contains a model with the specified charactername.
		/// </summary>
		/// <param name="characterName">The character name to check.</param>
		/// <returns>True if the name is taken.</returns>
		Task<bool> ContainsAsync(string characterName);
	}
}
