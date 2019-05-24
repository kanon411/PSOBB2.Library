using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;

namespace GladMMO
{
	public sealed class LocalTestNameQueryService : INameQueryService, INameQueryStorageable
	{
		private Dictionary<ObjectGuid, string> LocalNameMap { get; } = new Dictionary<ObjectGuid, string>(ObjectGuidEqualityComparer<ObjectGuid>.Instance);

		/// <inheritdoc />
		public void EnsureExists([NotNull] ObjectGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			if(!LocalNameMap.ContainsKey(entity))
				throw new KeyNotFoundException($"Entity: {entity} not found in {nameof(INameQueryService)}.");
		}

		/// <inheritdoc />
		public string Retrieve([NotNull] ObjectGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			return LocalNameMap[entity];
		}

		/// <inheritdoc />
		public Task<string> RetrieveAsync([NotNull] ObjectGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			return Task.FromResult(Retrieve(entity));
		}

		/// <inheritdoc />
		public void Add([NotNull] ObjectGuid entity, [NotNull] string name)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			LocalNameMap[entity] = name;
		}
	}
}
