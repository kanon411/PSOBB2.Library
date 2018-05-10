using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Guardians
{
	public sealed class DatabaseBackedCharacterRepository : ICharacterRepository
	{
		/// <summary>
		/// The database context.
		/// </summary>
		private CharacterDatabaseContext Context { get; }

		/// <inheritdoc />
		public DatabaseBackedCharacterRepository(CharacterDatabaseContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			Context = context;
		}

		public Task<bool> ContainsAsync(int key)
		{
			return Context
				.Characters
				.AnyAsync(c => c.CharacterId == key);
		}

		public Task<bool> ContainsAsync(string key)
		{
			return Context
				.Characters
				.AnyAsync(c => c.CharacterName == key);
		}

		/// <inheritdoc />
		public async Task<string> RetrieveNameAsync(int key)
		{
			if(!await ContainsAsync(key))
				throw new InvalidOperationException($"Requested Character with Key: {key} but none exist.");

			CharacterDatabaseModel character = await Context
				.Characters
				.FindAsync(key);

			if(character == null)
				throw new InvalidOperationException($"Tried to load Character with Key: {key} from database. Expected to exist but null.");

			return character.CharacterName;
		}

		public Task<CharacterDatabaseModel> RetrieveAsync(int key)
		{
			return Context
				.Characters
				.FindAsync(key);
		}

		public Task<CharacterDatabaseModel> RetrieveAsync(string key)
		{
			throw new NotImplementedException($"TODO: Implement name based retrieve");
		}

		public async Task<bool> TryCreateAsync(CharacterDatabaseModel model)
		{
			await Context.Characters.AddAsync(model);

			//Returns the amount of rows changed.
			//We expect more than 1 to indicate it was added successfully
			return 0 != await Context.SaveChangesAsync();
		}

		public Task<bool> TryDeleteAsync(int key)
		{
			throw new NotImplementedException($"TODO: Implement delete based on key.");
		}

		public Task<bool> TryDeleteAsync(string key)
		{
			throw new NotImplementedException($"TODO: Implement delete based on key.");
		}
	}
}
