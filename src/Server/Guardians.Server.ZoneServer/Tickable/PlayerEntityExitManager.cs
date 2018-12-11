using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nito.AsyncEx;

namespace Guardians
{
	public sealed class PlayerEntityExitManager : IGameTickable
	{
		/// <summary>
		/// The queue that contains sesssions that need to be cleaned up.
		/// </summary>
		private IDequeable<PlayerSessionDeconstructionContext> SessionCleanupQueue { get; }

		/// <summary>
		/// The deconstrutor for the session.
		/// </summary>
		private IObjectDestructorable<PlayerSessionDeconstructionContext> SessionDestructor { get; }

		/// <summary>
		/// The zone client game service.
		/// </summary>
		private IZoneServerToGameServerClient ZoneClientGameService { get; }


		private IConnectionEntityCollection ConnectionToEntityMap { get; }

		private GlobalEntityCollectionsLockingPolicy CollectionLockingPolicy { get; }

		/// <inheritdoc />
		public PlayerEntityExitManager(
			[NotNull] IDequeable<PlayerSessionDeconstructionContext> sessionCleanupQueue,
			[NotNull] IObjectDestructorable<PlayerSessionDeconstructionContext> sessionDestructor,
			[NotNull] IZoneServerToGameServerClient zoneClientGameService,
			[NotNull] IConnectionEntityCollection connectionToEntityMap,
			[NotNull] GlobalEntityCollectionsLockingPolicy collectionLockingPolicy)
		{
			SessionCleanupQueue = sessionCleanupQueue ?? throw new ArgumentNullException(nameof(sessionCleanupQueue));
			SessionDestructor = sessionDestructor ?? throw new ArgumentNullException(nameof(sessionDestructor));
			ZoneClientGameService = zoneClientGameService ?? throw new ArgumentNullException(nameof(zoneClientGameService));
			ConnectionToEntityMap = connectionToEntityMap ?? throw new ArgumentNullException(nameof(connectionToEntityMap));
			CollectionLockingPolicy = collectionLockingPolicy ?? throw new ArgumentNullException(nameof(collectionLockingPolicy));
		}

		/// <inheritdoc />
		public void Tick()
		{
			if(SessionCleanupQueue.isEmpty)
				return;

			//The reason this system is so simple is basically the other threads just need a way to queue up
			//cleanup on the main thread. The reasoning being that GameObject destruction needs to occur
			//as well as collection modification needs to happen, and the main thread is where the majority if collection
			//iteration should be taking place.
			PlayerSessionDeconstructionContext context = SessionCleanupQueue.Dequeue();

			NetworkEntityGuid entityGuid = ConnectionToEntityMap[context.ConnectionId];

			//We have to lock here because if we don't people may be iterating the collections.
			using(var lockObj = CollectionLockingPolicy.WriterLock(null, CancellationToken.None))
			{
				SessionDestructor.Destroy(context);
			}

			//We need to async send the release request, a very important part of session cleanup.
			//if this failes we have BIG problems. BIG BIG BIG.
			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				//TODO: We have a big problem if this fails, we need to handle it properly. Otherwise the player cannot log in again.
				ProjectVersionStage.AssertBeta();
				await ZoneClientGameService.ReleaseActiveSession(entityGuid.EntityId)
					.ConfigureAwait(false);
			});

			//TODO: This is a HACK We're in a sync context and we need to send a web request to the gameserver to remove this session.
			
		}
	}
}
