using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guardians
{
	public interface IEntityDataLockingService
	{
		/// <summary>
		/// Releases one interest unit for the provided
		/// entity guid <see cref="guid"/>.
		/// </summary>
		/// <param name="guid">The guid to release interest for.</param>
		/// <returns></returns>
		Task ReleaseEntityInterestAsync(NetworkEntityGuid guid);

		/// <summary>
		/// Aquires an increase of one interest unit for the provided
		/// entity guid <see cref="guid"/>.
		/// </summary>
		/// <param name="guid">The guid to increase interest for.</param>
		/// <returns></returns>
		Task RegisterEntityInterestAsync(NetworkEntityGuid guid);

		/// <summary>
		/// Indicates if the entity data locking service
		/// has a registeration for the provided entity <see cref="guid"/>.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns>True if a lock service is available for the entity.</returns>
		Task<bool> ContainsLockingServiceForAsync(NetworkEntityGuid guid);

		/// <summary>
		/// Attempts to aquire a lock for the provided <see cref="guid"/> <see cref="NetworkEntityGuid"/>.
		/// This is not object instance based. It is on internal value basis.
		/// </summary>
		/// <param name="guid">The Entity to lock for.</param>
		/// <returns>An awaitable and dispoable async lock object.</returns>
		Task<IDisposable> AquireEntityLockAsync(NetworkEntityGuid guid);
	}
}
