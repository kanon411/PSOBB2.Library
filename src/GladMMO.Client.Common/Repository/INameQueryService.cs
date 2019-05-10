using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladMMO
{
	public interface INameQueryService
	{
		/// <summary>
		/// Retrieves the name of the entity
		/// from the provided <see cref="id"/>.
		/// </summary>
		/// <param name="id">The id of the entity.</param>
		/// <exception cref="KeyNotFoundException">Throws if the key is not found.</exception>
		/// <returns>The name.</returns>
		string Retrieve(int id);

		/// <summary>
		/// Retrieves the name of the entity
		/// from the provided <see cref="id"/>.
		/// </summary>
		/// <param name="id">The id of the entity.</param>
		/// <exception cref="KeyNotFoundException">Throws if the key is not found.</exception> 
		/// <returns>The name.</returns>
		Task<string> RetrieveAsync(int id);
	}
}
