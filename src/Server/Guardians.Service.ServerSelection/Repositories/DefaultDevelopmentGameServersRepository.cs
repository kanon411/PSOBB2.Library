using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaloLive.Models.NameResolution;

namespace Guardians
{
	/// <summary>
	/// A quick development implementation of the
	/// <see cref="IGameServersStoreRepository"/> interface.
	/// </summary>
	public sealed class DefaultDevelopmentGameServersRepository : IGameServersStoreRepository
	{
		/// <inheritdoc />
		public async Task<GameServerEntry[]> RetriveServers()
		{
			//TODO: Add static domain for endpoint
			return new GameServerEntry[]
			{
				new GameServerEntry(new ResolvedEndpoint("todo", 80), GameServerStatusFlags.NewServer | GameServerStatusFlags.Online, "Development")
			};
		}
	}
}
