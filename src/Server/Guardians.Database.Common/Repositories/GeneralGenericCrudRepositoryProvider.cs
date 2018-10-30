using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Guardians
{
	public sealed class GeneralGenericCrudRepositoryProvider<TKey, TModelType> : IGenericRepositoryCrudable<TKey, TModelType> 
		where TModelType : class
	{
		private DbSet<TModelType> ModelSet { get; }

		private DbContext Context { get; }

		/// <inheritdoc />
		public GeneralGenericCrudRepositoryProvider(DbSet<TModelType> modelSet, DbContext context)
		{
			ModelSet = modelSet ?? throw new ArgumentNullException(nameof(modelSet));
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public async Task<bool> ContainsAsync(TKey key)
		{
			return await RetrieveAsync(key).ConfigureAwait(false) != null;
		}

		/// <inheritdoc />
		public async Task<bool> TryCreateAsync(TModelType model)
		{
			//TODO: Should we validate no key already exists?
			ModelSet.Add(model);
			return await SaveAndCheckResultsAsync()
				.ConfigureAwait(false);
		}

		private async Task<bool> SaveAndCheckResultsAsync()
		{
			return await Context.SaveChangesAsync().ConfigureAwait(false) != 0;
		}

		/// <inheritdoc />
		public Task<TModelType> RetrieveAsync(TKey key)
		{
			return ModelSet.FindAsync(key);
		}

		/// <inheritdoc />
		public async Task<bool> TryDeleteAsync(TKey key)
		{
			//If it doesn't exist then this will just fail, so get out soon.
			if(!await ContainsAsync(key).ConfigureAwait(false))
				return false;

			TModelType modelType = await RetrieveAsync(key)
				.ConfigureAwait(false);

			ModelSet.Remove(modelType);

			return await SaveAndCheckResultsAsync()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task UpdateAsync(TKey key, TModelType model)
		{
			if(!await ContainsAsync(key).ConfigureAwait(false))
				throw new InvalidOperationException($"Cannot update model with Key: {key} as it does not exist.");

			ModelSet.Update(model);

			await SaveAndCheckResultsAsync()
				.ConfigureAwait(false);
		}
	}
}
