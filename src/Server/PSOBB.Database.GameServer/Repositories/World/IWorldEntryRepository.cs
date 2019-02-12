using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PSOBB
{
	/// <summary>
	/// Contract for world entry repository types.
	/// </summary>
	public interface IWorldEntryRepository : IGenericRepositoryCrudable<long, WorldEntryModel>
	{
		Task SetWorldValidated(long worldId);
	}
}
