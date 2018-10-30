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

		private IReadonlyEntityGuidMappable<GameObject> KnownEntites { get; }

		/// <inheritdoc />
		public NetworkVisibilityChangeEventHandler(
			ILog logger,
			IFactoryCreatable<GameObject, DefaultEntityCreationContext> entityFactory,
			IObjectDestructorable<NetworkEntityGuid> entityDestructor, 
			IReadonlyEntityGuidMappable<GameObject> knownEntites) 
			: base(logger)
		{
			EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			KnownEntites = knownEntites;
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
				EntityFactory.Create(new DefaultEntityCreationContext(creationData.EntityGuid, creationData.InitialMovementData, ComputePrefabTypeFromGuid(creationData.EntityGuid)));

			foreach(var destroyData in payload.OutOfRangeEntities)
			{
				//If we don't know it, we likely encountered the rare edge case that is the result
				//of some hacks that were added to keep the wheels from falling off.
				//These will eventually be fixed, but for now we should just skip ones we don't know
				//because we can't remove what we don't know.
				if(!KnownEntites.ContainsKey(destroyData))
					continue;

				EntityDestructor.Destroy(destroyData);
			}

			//We need to spawn newly encountered entites.
			return Task.CompletedTask;
		}

		private EntityPrefab ComputePrefabTypeFromGuid(NetworkEntityGuid creationDataEntityGuid)
		{
			switch(creationDataEntityGuid.EntityType)
			{
				case EntityType.None:
					return EntityPrefab.Unknown;
				case EntityType.Player:
					return EntityPrefab.RemotePlayer;
				case EntityType.GameObject:
					return EntityPrefab.Unknown;
				case EntityType.Npc:
					return EntityPrefab.NetworkNpc;
			}

			return EntityPrefab.Unknown;
		}
	}
}