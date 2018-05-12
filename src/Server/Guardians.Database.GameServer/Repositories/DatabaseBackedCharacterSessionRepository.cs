using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Guardians
{
	
	public sealed class DatabaseBackedCharacterSessionRepository : ICharacterSessionRepository
	{
		private CharacterDatabaseContext Context { get; }

		/// <inheritdoc />
		public DatabaseBackedCharacterSessionRepository(CharacterDatabaseContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public async Task<bool> ContainsAsync(int key)
		{
			CheckAndThrowKey(key);

			return null != await Context
				.CharacterSessions
				.FindAsync(key);
		}

		/// <inheritdoc />
		public async Task<bool> TryCreateAsync(CharacterSessionModel model)
		{
			if(model == null) throw new ArgumentNullException(nameof(model));

			//TODO: look into transactions/locks
			await Context
				.CharacterSessions
				.AddAsync(model);
			
			//TODO: This can throw due to race condition
			int rowChangedCount = await Context.SaveChangesAsync();

			return rowChangedCount != 0;
		}

		/// <inheritdoc />
		public Task<CharacterSessionModel> RetrieveAsync(int key)
		{
			CheckAndThrowKey(key);

			return Context
				.CharacterSessions
				.FindAsync(key);
		}

		private static void CheckAndThrowKey(int key)
		{
			if(key < 0) throw new ArgumentOutOfRangeException(nameof(key));
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task<bool> AccountHasActiveSession(int accountId)
		{
			CheckAndThrowKey(accountId);

			return Context
				.CharacterSessions
				.AnyAsync(s => s.IsSessionActive && s.AccountId == accountId);
		}
	}
}
