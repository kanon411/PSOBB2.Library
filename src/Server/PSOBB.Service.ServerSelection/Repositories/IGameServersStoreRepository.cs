﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSOBB
{
	/// <summary>
	/// Contract for any repository that can provide information about
	/// game servers.
	/// </summary>
	public interface IGameServersStoreRepository
	{
		/// <summary>
		/// Loads all server entries from the repository.
		/// </summary>
		/// <returns>Retrives all servers.</returns>
		Task<GameServerEntry[]> RetriveServers();
	}
}
