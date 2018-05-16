using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for zone server entry data access.
	/// </summary>
	public interface IZoneServerRepository : IGenericRepositoryCrudable<int, ZoneInstanceEntryModel>
	{
		
	}
}
