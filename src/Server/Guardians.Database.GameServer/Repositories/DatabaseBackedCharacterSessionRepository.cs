﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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
				.ClaimedSession
				.AnyAsync(cs => cs.Session.CharacterEntry.AccountId == accountId);
		}

		/// <inheritdoc />
		public async Task<bool> TryClaimUnclaimedSession(int accountId, int characterId)
		{
			//TODO: Pomelo MySql doesn't support out values on stored proc see: https://github.com/mysql-net/MySqlConnector/issues/231
			/*MySqlParameter parameter = new MySqlParameter(@"@result", false)
			{
				Direction = ParameterDirection.Output
			};

			await Context.Database
				.ExecuteSqlCommandAsync($@"CALL create_claimed_session({accountId},{characterId},@result)", parameter);

			return (bool)parameter.Value;*/

			try
			{
				await Context.Database
					.ExecuteSqlCommandAsync($@"CALL create_claimed_session({accountId},{characterId})");

				return true;
			}
			catch(Exception)
			{
				//TODO: Log
				return false;
			}
		}
	}
}