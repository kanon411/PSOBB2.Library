using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for world entry repository types.
	/// </summary>
	public interface IWorldEntryRepository : IGenericRepositoryCrudable<long, WorldEntryModel>
	{
		
	}
}
