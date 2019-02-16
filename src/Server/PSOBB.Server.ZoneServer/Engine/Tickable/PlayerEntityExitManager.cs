using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Logging;
using Nito.AsyncEx;

namespace PSOBB
{
	[GameInitializableOrdering(Int32.MaxValue)] //this should ALWAYS be last, just incase anything else is in queue.
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class PlayerEntityExitManager : IGameTickable, ITickableSkippable
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

		private ILog Logger { get; }

		//If it's empty, we can just skip.
		/// <inheritdoc />
		public bool IsTickableSkippable => SessionCleanupQueue.isEmpty;

		/// <summary>
		/// The collections locking policy.
		/// </summary>
		private GlobalEntityCollectionsLockingPolicy LockingPolicy { get; }

		/// <inheritdoc />
		public PlayerEntityExitManager(
			[NotNull] IDequeable<PlayerSessionDeconstructionContext> sessionCleanupQueue,
			[NotNull] IObjectDestructorable<PlayerSessionDeconstructionContext> sessionDestructor,
			[NotNull] IZoneServerToGameServerClient zoneClientGameService,
			[NotNull] IConnectionEntityCollection connectionToEntityMap,
			[NotNull] ILog logger,
			[NotNull] GlobalEntityCollectionsLockingPolicy lockingPolicy)
		{
			SessionCleanupQueue = sessionCleanupQueue ?? throw new ArgumentNullException(nameof(sessionCleanupQueue));
			SessionDestructor = sessionDestructor ?? throw new ArgumentNullException(nameof(sessionDestructor));
			ZoneClientGameService = zoneClientGameService ?? throw new ArgumentNullException(nameof(zoneClientGameService));
			ConnectionToEntityMap = connectionToEntityMap ?? throw new ArgumentNullException(nameof(connectionToEntityMap));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LockingPolicy = lockingPolicy ?? throw new ArgumentNullException(nameof(lockingPolicy));
		}

		/// <inheritdoc />
		public void Tick()
		{
			if(SessionCleanupQueue.isEmpty)
				return;

			using(LockingPolicy.WriterLock(null, CancellationToken.None))
			{
				//The reason this system is so simple is basically the other threads just need a way to queue up
				//cleanup on the main thread. The reasoning being that GameObject destruction needs to occur
				//as well as collection modification needs to happen, and the main thread is where the majority if collection
				//iteration should be taking place.
				PlayerSessionDeconstructionContext context = SessionCleanupQueue.Dequeue();

				//it's possible that we're attempting to clean up an entity for a connection
				//that doesn't have one. This can happen if they disconnect during claim or before claim.
				if(!ConnectionToEntityMap.ContainsKey(context.ConnectionId))
				{
					if(Logger.IsInfoEnabled)
						Logger.Info($"ConnectionId: {context.ConnectionId} had entity exit cleanup but contained no entity. This is not an error.");

					//We may be in this method, handling cleanup for an entity that has a claim going on
					//in the claim handler, but is still awaiting a response for character data from the gameserver. MEANING we could end up with hanging entites.
					//This is not mitgated here, but inside the player entery factory
					//which SHOULD make a check for the connection still being valid AFTER creation
					//Not before because we still want to create, and then deconstruct. Reasoning being that gameserver session
					//will still be claimed unless we go through th cleanup process.
					return;
				}

				NetworkEntityGuid entityGuid = ConnectionToEntityMap[context.ConnectionId];

				SessionDestructor.Destroy(context);

				//We need to async send the release request, a very important part of session cleanup.
				//if this failes we have BIG problems. BIG BIG BIG.
				UnityExtended.UnityMainThreadContext.PostAsync(async () =>
				{
					//TODO: We have a big problem if this fails, we need to handle it properly. Otherwise the player cannot log in again.
					ProjectVersionStage.AssertBeta();
					await ZoneClientGameService.ReleaseActiveSession(entityGuid.EntityId)
						.ConfigureAwait(false);
				});
			}
		}
	}
}
