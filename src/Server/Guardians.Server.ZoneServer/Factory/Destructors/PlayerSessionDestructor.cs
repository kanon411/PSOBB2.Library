using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class PlayerSessionDestructor : IObjectDestructorable<PlayerSessionDeconstructionContext>
	{
		private IObjectDestructorable<PlayerEntityDestructionContext> PlayerEntityDestructor { get; }

		private IConnectionEntityCollection ConnectionToEntityMap { get; }

		private IEntityDataSaveable EntitySaver { get; }

		/// <inheritdoc />
		public PlayerSessionDestructor(
			[NotNull] IObjectDestructorable<PlayerEntityDestructionContext> playerEntityDestructor,
			[NotNull] IConnectionEntityCollection connectionToEntityMap,
			[NotNull] IEntityDataSaveable entitySaver)
		{
			PlayerEntityDestructor = playerEntityDestructor ?? throw new ArgumentNullException(nameof(playerEntityDestructor));
			ConnectionToEntityMap = connectionToEntityMap ?? throw new ArgumentNullException(nameof(connectionToEntityMap));
			EntitySaver = entitySaver ?? throw new ArgumentNullException(nameof(entitySaver));
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

			//TODO: Do this async, somehow.
			//Save the entity
			EntitySaver.Save(ConnectionToEntityMap[obj.ConnectionId]);

			//An entity exists, so we just pass it along but we also must remove it from the map afterwards.
			bool result = PlayerEntityDestructor.Destroy(new PlayerEntityDestructionContext(ConnectionToEntityMap[obj.ConnectionId]));

			ConnectionToEntityMap.Remove(obj.ConnectionId);

			return result;
		}
	}
}
