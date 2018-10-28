using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public sealed class DatabaseBackedNpcTemplateRepository : INpcTemplateRepository
	{
		private NpcDatabaseContext NpcDatabase { get; }

		/// <inheritdoc />
		public DatabaseBackedNpcTemplateRepository(NpcDatabaseContext npcDatabase)
		{
			NpcDatabase = npcDatabase ?? throw new ArgumentNullException(nameof(npcDatabase));
		}

		/// <inheritdoc />
		public async Task<bool> ContainsAsync(int key)
		{
			return (await NpcDatabase.Templates.FindAsync(key).ConfigureAwait(false)) != null;
		}

		/// <inheritdoc />
		public async Task<bool> TryCreateAsync(NPCTemplateModel model)
		{
			NpcDatabase.Templates.Add(model);

			return (await NpcDatabase.SaveChangesAsync().ConfigureAwait(false)) != 0;
		}

		/// <inheritdoc />
		public Task<NPCTemplateModel> RetrieveAsync(int key)
		{
			return NpcDatabase.Templates.FindAsync(key);
		}

		/// <inheritdoc />
		public async Task<bool> TryDeleteAsync(int key)
		{
			if(!await ContainsAsync(key).ConfigureAwait(false))
				return false;

			NpcDatabase.Templates.Remove(await RetrieveAsync(key));

			return (await NpcDatabase.SaveChangesAsync().ConfigureAwait(false)) != 0;
		}

		/// <inheritdoc />
		public async Task<string> RetrieveNameAsync(int key)
		{
			if(!await ContainsAsync(key).ConfigureAwait(false))
				throw new KeyNotFoundException($"The provided Key: {key} Not Found in {nameof(NPCEntryModel)} table.");

			NPCTemplateModel model = await RetrieveAsync(key)
				.ConfigureAwait(false);

			if(model == null)
				throw new InvalidOperationException($"Failed to load {nameof(NPCEntryModel)} for Key: {key}");

			return model.NpcName;
		}
	}
}
