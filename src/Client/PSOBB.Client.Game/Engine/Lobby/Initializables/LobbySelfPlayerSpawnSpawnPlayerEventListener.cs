using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	//The name might be confusing, but it just an event listener
	//that spawns the player on selfspawnevent.
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class LobbySelfPlayerSpawnSpawnPlayerEventListener : BaseSingleEventListenerInitializable<ISelfPlayerSpawnEventSubscribable, SelfPlayerSpawnEventArgs>
	{
		private IFactoryCreatable<GameObject, DefaultEntityCreationContext> PlayerFactory { get; }

		/// <inheritdoc />
		public LobbySelfPlayerSpawnSpawnPlayerEventListener([NotNull] ISelfPlayerSpawnEventSubscribable subscriptionService, [NotNull] IFactoryCreatable<GameObject, DefaultEntityCreationContext> playerFactory) 
			: base(subscriptionService)
		{
			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, SelfPlayerSpawnEventArgs args)
		{
			EntityFieldDataCollection<EntityDataFieldType> entityData = CreateEntityDataCollectionFromPayload(args.CreationData.InitialFieldValues);

			//Don't do any checks for now, we just spawn
			GameObject playerGameObject = PlayerFactory.Create(new DefaultEntityCreationContext(args.CreationData.EntityGuid, args.CreationData.InitialMovementData, EntityPrefab.LocalPlayer, entityData));
		}

		private static EntityFieldDataCollection<EntityDataFieldType> CreateEntityDataCollectionFromPayload(FieldValueUpdate fieldUpdateValues)
		{
			//TODO: We need better initial handling for entity data
			EntityFieldDataCollection<EntityDataFieldType> entityData = new EntityFieldDataCollection<EntityDataFieldType>();

			int currentSentIndex = 0;
			foreach(var setIndex in fieldUpdateValues.FieldValueUpdateMask.EnumerateSetBitsByIndex())
			{
				entityData.SetFieldValue(setIndex, fieldUpdateValues.FieldValueUpdates.ElementAt(currentSentIndex));
				currentSentIndex++;
			}

			return entityData;
		}
	}
}
