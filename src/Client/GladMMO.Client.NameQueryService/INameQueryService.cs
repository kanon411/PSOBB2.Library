using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;

namespace GladMMO
{
	public interface INameQueryService
	{
		/// <summary>
		/// Retrieves the name of the entity
		/// from the provided <see cref="entity"/>.
		/// </summary>
		/// <param name="entity">The id of the entity.</param>
		/// <exception cref="KeyNotFoundException">Throws if the key is not found.</exception> 
		/// <returns>The name.</returns>
		Task<string> RetrieveAsync(NetworkEntityGuid entity);
	}
}
