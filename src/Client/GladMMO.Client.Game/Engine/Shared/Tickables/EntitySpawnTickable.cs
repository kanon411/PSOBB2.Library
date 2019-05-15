using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;
using UnityEngine;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ILocalPlayerSpawnedEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.DefaultLobby)]
	public sealed class EntitySpawnTickable : EventQueueBasedTickable<INetworkEntityVisibleEventSubscribable, NetworkEntityNowVisibleEventArgs>, ILocalPlayerSpawnedEventSubscribable
	{
		//private IFactoryCreatable<GameObject, DefaultEntityCreationContext> EntityFactory { get; }

		private IKnownEntitySet KnownEntites { get; }

		//TODO: This is just for testing, w
		private ICharacterDataRepository CharacterDataRepo { get; }

		//TODO: JUst for testing.
		/// <inheritdoc />
		public event EventHandler<LocalPlayerSpawnedEventArgs> OnLocalPlayerSpawned;

		//TODO: Just for testing.
		private ILocalPlayerDetails LocalPlayerDetails { get; }

		/// <inheritdoc />
		public EntitySpawnTickable([NotNull] INetworkEntityVisibleEventSubscribable subscriptionService, 
			[NotNull] ILog logger,
			[NotNull] IKnownEntitySet knownEntites,
			[NotNull] ICharacterDataRepository characterDataRepo,
			[NotNull] ILocalPlayerDetails localPlayerDetails)
			: base(subscriptionService, true, logger) //TODO: We probably shouldn't spawn everything per frame. We should probably stagger spawning.
		{
			KnownEntites = knownEntites ?? throw new ArgumentNullException(nameof(knownEntites));
			CharacterDataRepo = characterDataRepo ?? throw new ArgumentNullException(nameof(characterDataRepo));
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
			//EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
		}

		/// <inheritdoc />
		protected override void HandleEvent(NetworkEntityNowVisibleEventArgs args)
		{
			try
			{
				//TODO: We need to do abit MORE about this, to know the entity.
				KnownEntites.AddEntity(args.EntityGuid);
				LocalPlayerDetails.LocalPlayerGuid = args.EntityGuid; //TODO: For testing! Remove othgerwise player looks like ALL objects.

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Entity: {args.EntityGuid.ObjectType}:{args.EntityGuid.CurrentObjectGuid} is now known.");

				if(CharacterDataRepo.CharacterId == args.EntityGuid.CurrentObjectGuid)
					OnLocalPlayerSpawned?.Invoke(this, new LocalPlayerSpawnedEventArgs(args.EntityGuid));

				//GameObject entityRootObject = EntityFactory.Create(new DefaultEntityCreationContext(args.CreationData.EntityGuid, args.CreationData.InitialMovementData, ComputePrefabTypeFromGuid(args.EntityGuid), args.EntityDataContainer));
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Create Entity: {args.EntityGuid} Exception: {e.Message}\n\nStack: {e.StackTrace}");
				throw;
			}
		}

		private EntityPrefab ComputePrefabTypeFromGuid(ObjectGuid creationDataEntityGuid)
		{
			switch(creationDataEntityGuid.ObjectType)
			{
				case EntityGuidMask.Instance:
					return EntityPrefab.Unknown;
				case EntityGuidMask.Player:
					return EntityPrefab.RemotePlayer;
				case EntityGuidMask.GameObject:
					return EntityPrefab.Unknown;
				case EntityGuidMask.Unit:
					return EntityPrefab.NetworkNpc;
			}

			return EntityPrefab.Unknown;
		}
	}
}
