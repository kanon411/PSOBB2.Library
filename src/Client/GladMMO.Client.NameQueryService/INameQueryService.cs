using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;
using Refit;

namespace GladMMO
{
	[Headers("User-Agent: GuardiansClient")]
	public interface INameQueryService
	{
		/// <summary>
		/// Retrieves the name of the entity
		/// from the provided <see cref="entity"/>.
		/// </summary>
		/// <param name="entity">The id of the entity.</param>
		/// <returns>Result of the namequery.</returns>
		[Headers("Cache-Control: max-age=600")]
		[Get("/api/namequery")]
		Task<NameQueryResponse> RetrieveAsync([JsonBody] NetworkEntityGuid entity);
	}
}
