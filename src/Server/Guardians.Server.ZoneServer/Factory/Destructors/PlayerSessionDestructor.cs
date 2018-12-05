using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Nito.AsyncEx;

namespace Guardians
{
	public sealed class PlayerSessionDestructor : IObjectDestructorable<PlayerSessionDeconstructionContext>
	{
		private IObjectDestructorable<PlayerEntityDestructionContext> PlayerEntityDestructor { get; }

		private IConnectionEntityCollection ConnectionToEntityMap { get; }

		private IEntityDataSaveable EntitySaver { get; }

		private IZoneServerToGameServerClient GameServerClient { get; }

		/// <inheritdoc />
		public PlayerSessionDestructor(
			[NotNull] IObjectDestructorable<PlayerEntityDestructionContext> playerEntityDestructor,
			[NotNull] IConnectionEntityCollection connectionToEntityMap,
			[NotNull] IEntityDataSaveable entitySaver,
			[NotNull] IZoneServerToGameServerClient gameServerClient)
		{
			PlayerEntityDestructor = playerEntityDestructor ?? throw new ArgumentNullException(nameof(playerEntityDestructor));
			ConnectionToEntityMap = connectionToEntityMap ?? throw new ArgumentNullException(nameof(connectionToEntityMap));
			EntitySaver = entitySaver ?? throw new ArgumentNullException(nameof(entitySaver));
			GameServerClient = gameServerClient ?? throw new ArgumentNullException(nameof(gameServerClient));
		}

		/// <inheritdoc />
		public bool Destroy([NotNull] PlayerSessionDeconstructionContext obj)
		{
			if(obj == null) throw new ArgumentNullException(nameof(obj));

			//The idea here is that the destructor just checks to see if
			//an entity is owned by this session/connection. If it is then it
			//will send the guid off to the real service that does destruction.
			if(!ConnectionToEntityMap.ContainsKey(obj.ConnectionId))
				return false;

			NetworkEntityGuid entityGuid = ConnectionToEntityMap[obj.ConnectionId];

			//TODO: Do this async, somehow.
			//Save the entity
			EntitySaver.Save(entityGuid);

			//An entity exists, so we just pass it along but we also must remove it from the map afterwards.
			bool result = PlayerEntityDestructor.Destroy(new PlayerEntityDestructionContext(entityGuid));

			ConnectionToEntityMap.Remove(obj.ConnectionId);

			//TODO: This is a HACK We're in a sync context and we need to send a web request to the gameserver to remove this session.
			ProjectVersionStage.AssertAlpha();
			AsyncContext.Run(async () =>
			{
				//TODO: This could technically fail, but there is nothing we could do really to save it.
				await GameServerClient.ReleaseActiveSession(entityGuid.EntityId)
					.ConfigureAwait(false);
			});

			return result;
		}
	}
}
