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
	//[SceneTypeCreateGladMMO(GameSceneType.DefaultLobby)]
	public sealed class EntitySpawnTickable : EventQueueBasedTickable<INetworkEntityVisibleEventSubscribable, NetworkEntityNowVisibleEventArgs>
	{
		//private IFactoryCreatable<GameObject, DefaultEntityCreationContext> EntityFactory { get; }

		/// <inheritdoc />
		public EntitySpawnTickable([NotNull] INetworkEntityVisibleEventSubscribable subscriptionService, [NotNull] ILog logger
			/*[NotNull] IFactoryCreatable<GameObject, DefaultEntityCreationContext> entityFactory*/)
			: base(subscriptionService, true, logger) //TODO: We probably shouldn't spawn everything per frame. We should probably stagger spawning.
		{
			//EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
		}

		/// <inheritdoc />
		protected override void HandleEvent(NetworkEntityNowVisibleEventArgs args)
		{
			try
			{
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
