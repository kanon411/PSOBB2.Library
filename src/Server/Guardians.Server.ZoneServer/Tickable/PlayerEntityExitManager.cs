using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nito.AsyncEx;

namespace Guardians
{
	[CollectionsLocking(LockType.Write)]
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

		//If it's empty, we can just skip.
		/// <inheritdoc />
		public bool IsTickableSkippable => SessionCleanupQueue.isEmpty;

		/// <inheritdoc />
		public PlayerEntityExitManager(
			[NotNull] IDequeable<PlayerSessionDeconstructionContext> sessionCleanupQueue,
			[NotNull] IObjectDestructorable<PlayerSessionDeconstructionContext> sessionDestructor,
			[NotNull] IZoneServerToGameServerClient zoneClientGameService,
			[NotNull] IConnectionEntityCollection connectionToEntityMap)
		{
			SessionCleanupQueue = sessionCleanupQueue ?? throw new ArgumentNullException(nameof(sessionCleanupQueue));
			SessionDestructor = sessionDestructor ?? throw new ArgumentNullException(nameof(sessionDestructor));
			ZoneClientGameService = zoneClientGameService ?? throw new ArgumentNullException(nameof(zoneClientGameService));
			ConnectionToEntityMap = connectionToEntityMap ?? throw new ArgumentNullException(nameof(connectionToEntityMap));
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
