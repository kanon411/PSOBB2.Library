using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace Guardians
{
	public sealed class NetworkVisibilityChangeEventHandler : BaseZoneClientGameMessageHandler<NetworkObjectVisibilityChangeEventPayload>
	{
		private IFactoryCreatable<GameObject, DefaultEntityCreationContext> EntityFactory { get; }

		private IObjectDestructorable<NetworkEntityGuid> EntityDestructor { get; }

		/// <inheritdoc />
		public NetworkVisibilityChangeEventHandler(
			ILog logger,
			IFactoryCreatable<GameObject, DefaultEntityCreationContext> entityFactory,
			IObjectDestructorable<NetworkEntityGuid> entityDestructor) 
			: base(logger)
		{
			EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, NetworkObjectVisibilityChangeEventPayload payload)
		{
			foreach(var entity in payload.EntitiesToCreate)
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Encountered new entity: {entity.EntityGuid}");
			}

			foreach(var entity in payload.OutOfRangeEntities)
			{
				if(Logger.IsErrorEnabled)
					Logger.Debug($"Leaving entity: {entity}");
			}

			//Assume it's a player for now
			foreach(var creationData in payload.EntitiesToCreate)
				EntityFactory.Create(new DefaultEntityCreationContext(creationData.EntityGuid, creationData.InitialMovementData, EntityPrefab.RemotePlayer));

			foreach(var destroyData in payload.OutOfRangeEntities)
				EntityDestructor.Destroy(destroyData);

			//We need to spawn newly encountered entites.
			return Task.CompletedTask;
		}
	}
}
