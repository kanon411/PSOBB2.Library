using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	/// <summary>
	/// Contract for zone server entry data access.
	/// </summary>
	public interface IZoneServerRepository : IGenericRepositoryCrudable<int, ZoneInstanceEntryModel>
	{
		/// <summary>
		/// Retrieves the zone entry by guid. 
		/// </summary>
		/// <param name="zoneGuid">The guid.</param>
		/// <returns></returns>
		Task<ZoneInstanceEntryModel> RetrieveByGuidAsync(Guid zoneGuid);
	}
}
