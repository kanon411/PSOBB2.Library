using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;

namespace GladMMO
{
	public sealed class LocalTestNameQueryService : INameQueryService, INameQueryStorageable
	{
		private Dictionary<NetworkEntityGuid, string> LocalNameMap { get; } = new Dictionary<NetworkEntityGuid, string>(NetworkGuidEqualityComparer<NetworkEntityGuid>.Instance);

		/// <inheritdoc />
		public void EnsureExists([NotNull] NetworkEntityGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			if(!LocalNameMap.ContainsKey(entity))
				throw new KeyNotFoundException($"Entity: {entity} not found in {nameof(INameQueryService)}.");
		}

		/// <inheritdoc />
		public string Retrieve([NotNull] NetworkEntityGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			return LocalNameMap[entity];
		}

		/// <inheritdoc />
		public Task<string> RetrieveAsync([NotNull] NetworkEntityGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			return Task.FromResult(Retrieve(entity));
		}

		/// <inheritdoc />
		public void Add([NotNull] NetworkEntityGuid entity, [NotNull] string name)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			LocalNameMap[entity] = name;
		}
	}
}
