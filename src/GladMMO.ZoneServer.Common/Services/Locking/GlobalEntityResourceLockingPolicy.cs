﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nito.AsyncEx;

namespace GladMMO
{
	public sealed class GlobalEntityResourceLockingPolicy : IContextualResourceLockingPolicy<NetworkEntityGuid>
	{
		static GlobalEntityResourceLockingPolicy()
		{
			//TODO: There is kinda data race here, we need to a global for this collection too to prevent it being modidified before the lock occurs inbetween lookup
			ProjectVersionStage.AssertBeta();
		}

		private IReadonlyEntityGuidMappable<AsyncReaderWriterLock> EntityAsyncLockMap { get; }

		/// <inheritdoc />
		public GlobalEntityResourceLockingPolicy([NotNull] IReadonlyEntityGuidMappable<AsyncReaderWriterLock> entityAsyncLockMap)
		{
			EntityAsyncLockMap = entityAsyncLockMap ?? throw new ArgumentNullException(nameof(entityAsyncLockMap));
		}

		/// <inheritdoc />
		public IDisposable ReaderLock(NetworkEntityGuid context, CancellationToken cancellationToken)
		{
			ThrowIfNoEntityInMap(context);

			return EntityAsyncLockMap[context].ReaderLock(cancellationToken);
		}

		private void ThrowIfNoEntityInMap(NetworkEntityGuid context)
		{
			//TODO: Race condition since we aren't locking the collection from modification.
			if(!EntityAsyncLockMap.ContainsKey(context))
				ThrowNoEntityInMap(context);
		}

		private static void ThrowNoEntityInMap(NetworkEntityGuid context)
		{
			throw new InvalidOperationException($"Cannot aquire lock on Entity: {context} as no lock data in the map.");
		}

		/// <inheritdoc />
		public AwaitableDisposable<IDisposable> ReaderLockAsync(NetworkEntityGuid context, CancellationToken cancellationToken)
		{
			ThrowIfNoEntityInMap(context);

			return EntityAsyncLockMap[context].ReaderLockAsync(cancellationToken);
		}

		/// <inheritdoc />
		public IDisposable WriterLock(NetworkEntityGuid context, CancellationToken cancellationToken)
		{
			ThrowIfNoEntityInMap(context);

			return EntityAsyncLockMap[context].WriterLock(cancellationToken);
		}

		/// <inheritdoc />
		public AwaitableDisposable<IDisposable> WriterLockAsync(NetworkEntityGuid context, CancellationToken cancellationToken)
		{
			ThrowIfNoEntityInMap(context);

			return EntityAsyncLockMap[context].WriterLockAsync(cancellationToken);
		}
	}
}
