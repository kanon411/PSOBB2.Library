﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladMMO
{
	public sealed class CacheableEntityNameQueryable : IEntityNameQueryable
	{
		private Dictionary<NetworkEntityGuid, string> LocalNameMap { get; } = new Dictionary<NetworkEntityGuid, string>(NetworkGuidEqualityComparer<NetworkEntityGuid>.Instance);

		private INameQueryService NameServiceQueryable { get; }

		private AsyncReaderWriterLock SyncObj { get; } = new AsyncReaderWriterLock();

		/// <inheritdoc />
		public CacheableEntityNameQueryable([NotNull] INameQueryService nameServiceQueryable)
		{
			NameServiceQueryable = nameServiceQueryable ?? throw new ArgumentNullException(nameof(nameServiceQueryable));
		}

		/// <inheritdoc />
		public void EnsureExists([NotNull] NetworkEntityGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			using(SyncObj.ReaderLock())
				if(!LocalNameMap.ContainsKey(entity))
					throw new KeyNotFoundException($"Entity: {entity} not found in {nameof(INameQueryService)}.");
		}

		/// <inheritdoc />
		public string Retrieve([NotNull] NetworkEntityGuid entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			using(SyncObj.ReaderLock())
			{
				return LocalNameMap[entity];
			}
		}

		/// <inheritdoc />
		public async Task<NameQueryResponse> RetrieveAsync(ulong rawGuidValue)
		{
			NetworkEntityGuid entity = new NetworkEntityGuid(rawGuidValue);

			using(await SyncObj.ReaderLockAsync())
			{
				if(LocalNameMap.ContainsKey(entity))
					return new NameQueryResponse(LocalNameMap[entity]); //do not call Retrieve, old versions of Unity3D don't support recursive readwrite locking.
			}

			//If we're here, it wasn't contained
			var result = await NameServiceQueryable.RetrieveAsync(entity);

			if(!result.isSuccessful)
				throw new InvalidOperationException($"Failed to query name for Entity: {entity}. Result: {result.ResultCode}.");

			//Add it
			using(await SyncObj.WriterLockAsync())
			{
				//Check if some other thing already initialized it

				if(LocalNameMap.ContainsKey(entity))
					return new NameQueryResponse(LocalNameMap[entity]); //do not call Retrieve, old versions of Unity3D don't support recursive readwrite locking.

				return new NameQueryResponse(LocalNameMap[entity] = result.EntityName);
			}
		}
	}
}
