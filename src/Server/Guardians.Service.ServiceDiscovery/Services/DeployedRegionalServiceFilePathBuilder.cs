using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaloLive.Network.Common;

namespace Guardians
{
	/// <summary>
	/// Local config path builder for the region endpoints.
	/// </summary>
	public sealed class DeployedRegionalServiceFilePathBuilder : IRegionalServiceFilePathBuilder
	{
		/// <inheritdoc />
		public string BuildPath(ClientRegionLocale region)
		{
			if(!Enum.IsDefined(typeof(ClientRegionLocale), region)) throw new ArgumentOutOfRangeException(nameof(region), "Value should be defined in the ClientRegionLocale enum.");

			return $@"Endpoints/Endpoints{region.ToString()}.json";
		}
	}
}
