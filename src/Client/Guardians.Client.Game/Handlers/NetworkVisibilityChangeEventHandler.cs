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

		private IReadonlyNetworkTimeService TimeService { get; }

		/// <summary>
		/// The voice gateway for player entities to enter.
		/// </summary>
		private IVoiceGateway VoiceGateway { get; }

		/// <inheritdoc />
		public NetworkVisibilityChangeEventHandler(
			ILog logger,
			IFactoryCreatable<GameObject, DefaultEntityCreationContext> entityFactory,
			IObjectDestructorable<NetworkEntityGuid> entityDestructor,
			IReadonlyEntityGuidMappable<GameObject> knownEntites,
			IReadonlyNetworkTimeService timeService, 
			IVoiceGateway voiceGateway)
			: base(logger)
		{
			EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			KnownEntites = knownEntites ?? throw new ArgumentNullException(nameof(knownEntites));
			TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
			VoiceGateway = voiceGateway;
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
			{
				//TODO: Right now we're creating a temporary entity data collection.
				EntityFieldDataCollection<EntityDataFieldType> testData = new EntityFieldDataCollection<EntityDataFieldType>();

				try
				{
					EntityFactory.Create(new DefaultEntityCreationContext(creationData.EntityGuid, GetInitialMovementData(creationData), ComputePrefabTypeFromGuid(creationData.EntityGuid), testData));
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Create Entity: {creationData.EntityGuid} Exception: {e.Message}\n\nStack: {e.StackTrace}");
					throw;
				}

				//Join players into the voice gateway
				if(creationData.EntityGuid.EntityType == EntityType.Player)
					VoiceGateway.JoinVoiceSession(creationData.EntityGuid);
			}

			foreach(var destroyData in payload.OutOfRangeEntities)
			{
				//If we don't know it, we likely encountered the rare edge case that is the result
				//of some hacks that were added to keep the wheels from falling off.
				//These will eventually be fixed, but for now we should just skip ones we don't know
				//because we can't remove what we don't know.
				if(!KnownEntites.ContainsKey(destroyData))
					continue;

				EntityDestructor.Destroy(destroyData);

				//Join players into the voice gateway
				if(destroyData.EntityType == EntityType.Player)
					VoiceGateway.LeaveVoiceSession(destroyData);
			}

			//We need to spawn newly encountered entites.
			return Task.CompletedTask;
		}

		private IMovementData GetInitialMovementData(EntityCreationData creationData)
		{
			//TODO: Remove path debug testing.
			//If it's not a player, let's return a debug path data.
			/*if(creationData.EntityGuid.EntityType != EntityType.Player)
				return new PathBasedMovementData(new Vector3[]
				{
					new Vector3(2,3,4),
					new Vector3(-5, 1, 5), 
					new Vector3(-5, 1, 17), 
					new Vector3(10, 1, 17), 
					new Vector3(10, 1, 5) 
				}, TimeService.CurrentRemoteTime); //TODO: This only works on local*/

			return creationData.InitialMovementData;
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