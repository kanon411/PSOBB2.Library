using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public sealed class DatabaseBackedWaypointsRepository : IWaypointsRepository
	{
		private GeneralGenericCrudRepositoryProvider<PathWaypointKey, PathWaypointModel> WaypointCrudProvider { get; }

		private CommonGameDatabaseContext Context { get; }

		public DatabaseBackedWaypointsRepository(CommonGameDatabaseContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			WaypointCrudProvider = new PathWaypointsCrudProvider(context.PathWaypoints, context);
		}

		/// <inheritdoc />
		public Task<bool> ContainsAsync(PathWaypointKey key)
		{
			return WaypointCrudProvider.ContainsAsync(key);
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(PathWaypointModel model)
		{
			return WaypointCrudProvider.TryCreateAsync(model);
		}

		/// <inheritdoc />
		public Task<PathWaypointModel> RetrieveAsync(PathWaypointKey key)
		{
			return WaypointCrudProvider.RetrieveAsync(key);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(PathWaypointKey key)
		{
			return WaypointCrudProvider.TryDeleteAsync(key);
		}

		/// <inheritdoc />
		public Task UpdateAsync(PathWaypointKey key, PathWaypointModel model)
		{
			return WaypointCrudProvider.UpdateAsync(key, model);
		}
	}
}
