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

	}
}
