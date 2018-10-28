using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public sealed class DatabaseBackedNpcEntryRepository : IGenericRepositoryCrudable<int, NPCEntryModel>
	{
		private NpcDatabaseContext NpcDatabase { get; }

		/// <inheritdoc />
		public DatabaseBackedNpcEntryRepository(NpcDatabaseContext npcDatabase)
		{
			NpcDatabase = npcDatabase ?? throw new ArgumentNullException(nameof(npcDatabase));
		}

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key)
		{
			var generalCrudProvider = BuildGeneralCrudProvider();

			return generalCrudProvider.ContainsAsync(key);
		}

		private GeneralGenericCrudRepositoryProvider<int, NPCEntryModel> BuildGeneralCrudProvider()
		{
			return new GeneralGenericCrudRepositoryProvider<int, NPCEntryModel>(NpcDatabase.Entries, NpcDatabase);
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(NPCEntryModel model)
		{
			var generalCrudProvider = BuildGeneralCrudProvider();

			return generalCrudProvider.TryCreateAsync(model);
		}

		/// <inheritdoc />
		public Task<NPCEntryModel> RetrieveAsync(int key)
		{
			var generalCrudProvider = BuildGeneralCrudProvider();

			return generalCrudProvider.RetrieveAsync(key);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key)
		{
			var generalCrudProvider = BuildGeneralCrudProvider();

			return generalCrudProvider.TryDeleteAsync(key);
		}
	}
}
