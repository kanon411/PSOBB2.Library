using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;
using JetBrains.Annotations;
//using PostSharp.Patterns.Caching;

namespace GladMMO
{
	//[CacheConfiguration(AbsoluteExpiration = 5, IgnoreThisParameter = true)]
	public sealed class CachedNameQueryServiceDecorator : INameQueryService
	{
		private INameQueryService NameQueryService { get; }

		/// <inheritdoc />
		public CachedNameQueryServiceDecorator(INameQueryService nameQueryService)
		{
			NameQueryService = nameQueryService ?? throw new ArgumentNullException(nameof(nameQueryService));
		}

		/// <inheritdoc />
		public void EnsureExists(NetworkEntityGuid entity)
		{
			NameQueryService.EnsureExists(entity);
		}

		/// <inheritdoc />
		//[Cache]
		public string Retrieve(NetworkEntityGuid entity)
		{
			return NameQueryService.Retrieve(entity);
		}

		/// <inheritdoc />
		//[Cache]
		public async Task<string> RetrieveAsync(NetworkEntityGuid entity)
		{
			return await NameQueryService.RetrieveAsync(entity)
				.ConfigureAwait(false);
		}
	}
}
