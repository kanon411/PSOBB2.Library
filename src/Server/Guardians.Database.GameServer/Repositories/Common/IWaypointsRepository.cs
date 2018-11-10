using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IWaypointsRepository
		: IGenericRepositoryCrudable<PathWaypointKey, PathWaypointModel>
	{
		
	}
}
