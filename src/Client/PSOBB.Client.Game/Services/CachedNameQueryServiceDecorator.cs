using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
		//[Cache]
		public string Retrieve(int id)
		{
			return NameQueryService.Retrieve(id);
		}

		/// <inheritdoc />
		//[Cache]
		public async Task<string> RetrieveAsync(int id)
		{
			return await NameQueryService.RetrieveAsync(id)
				.ConfigureAwait(false);
		}
	}
}
