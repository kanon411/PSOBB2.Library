using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Guardians
{
	public sealed class DatabaseBackedZoneServerRepository : IZoneServerRepository, IGenericRepositoryCrudable<Guid, ZoneInstanceEntryModel>
	{
		private CharacterDatabaseContext Context { get; }

		/// <inheritdoc />
		public DatabaseBackedZoneServerRepository(CharacterDatabaseContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key)
		{
			return Context
				.ZoneEntries
				.AnyAsync(z => z.ZoneId == key);
		}

		/// <inheritdoc />
		public Task<bool> ContainsAsync(Guid key)
		{
			return Context
				.ZoneEntries
				.AnyAsync(z => z.ZoneGuid == key);
		}

		/// <inheritdoc />
		public async Task<bool> TryCreateAsync(ZoneInstanceEntryModel model)
		{
#pragma warning disable AsyncFixer02 // Long running or blocking operations under an async method
			Context
				.ZoneEntries
				.Add(model);
#pragma warning restore AsyncFixer02 // Long running or blocking operations under an async method

			return 0 != await Context.SaveChangesAsync();
		}

		/// <inheritdoc />
		public Task<ZoneInstanceEntryModel> RetrieveAsync(Guid key)
		{
			return Context
				.ZoneEntries
				.FirstAsync(z => z.ZoneGuid == key);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task UpdateAsync(Guid key, ZoneInstanceEntryModel model)
		{
			//Since the generic crud provider will use Find we can't use it
			//with are secondary name key. We have to implement this manually
			if(!await Context.ZoneEntries.AnyAsync(z => z.ZoneGuid == key).ConfigureAwait(false))
				throw new InvalidOperationException($"Cannot update model with Key: {key} as it does not exist.");

			Context.ZoneEntries.Update(model);

			await Context.SaveChangesAsync()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public Task<ZoneInstanceEntryModel> RetrieveAsync(int key)
		{
			return Context
				.ZoneEntries
				.FindAsync(key);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task UpdateAsync(int key, ZoneInstanceEntryModel model)
		{
			GeneralGenericCrudRepositoryProvider<int, ZoneInstanceEntryModel> crudProvider = new GeneralGenericCrudRepositoryProvider<int, ZoneInstanceEntryModel>(Context.ZoneEntries, Context);

			return crudProvider.UpdateAsync(key, model);
		}

		/// <inheritdoc />
		public Task<ZoneInstanceEntryModel> RetrieveByGuidAsync(Guid zoneGuid)
		{
			return RetrieveAsync(zoneGuid);
		}
	}
}
