﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PSOBB
{
	public interface ICharacterRepository : 
		IGenericRepositoryCrudable<int, CharacterEntryModel>, INameQueryableRepository<int>
	{
		/// <summary>
		/// Checks if the repository contains a model with the specified charactername.
		/// </summary>
		/// <param name="characterName">The character name to check.</param>
		/// <returns>True if the name is taken.</returns>
		Task<bool> ContainsAsync(string characterName);

		/// <summary>
		/// Tries to load all the characters with the provided <see cref="accountId"/>.
		/// If none exist it will produce an empty collection.
		/// </summary>
		/// <param name="accountId">The account id to check.</param>
		/// <returns></returns>
		Task<int[]> CharacterIdsForAccountId(int accountId);
	}
}
