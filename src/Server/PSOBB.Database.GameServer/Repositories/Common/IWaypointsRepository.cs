using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PSOBB
{
	public interface IWaypointsRepository
		: IGenericRepositoryCrudable<PathWaypointKey, PathWaypointModel>
	{
		Task<bool> ContainsPathAsync(int pathId);

		Task<IReadOnlyCollection<PathWaypointModel>> RetrievePointsFromPathAsync(int pathId);
	}
}
