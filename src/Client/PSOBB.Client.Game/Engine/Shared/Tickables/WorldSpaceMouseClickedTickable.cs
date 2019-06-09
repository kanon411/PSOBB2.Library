using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class WorldSpaceMouseClickedTickable : IGameTickable, IGameInitializable
	{
		private Camera CameraReference { get; set; }

		//TODO: Is this large enough for all hits?
		private readonly RaycastHit[] CachedHitResults = new RaycastHit[20];

		private IReadonlyGameObjectToEntityMappable GameObjectToEntityMappable { get; }

		/// <inheritdoc />
		public WorldSpaceMouseClickedTickable([NotNull] IReadonlyGameObjectToEntityMappable gameObjectToEntityMappable)
		{
			GameObjectToEntityMappable = gameObjectToEntityMappable ?? throw new ArgumentNullException(nameof(gameObjectToEntityMappable));
		}

		/// <inheritdoc />
		public void Tick()
		{
			if(Input.GetMouseButtonDown(0))
			{
				Ray ray = CameraReference.ScreenPointToRay(Input.mousePosition);

				//TODO: We should enumerate the layers
				//5 is UI right now.
				int resultCount = Physics.RaycastNonAlloc(ray, CachedHitResults, 1000.0f, 1 << 5); //5th

				if(resultCount == 0)
				{
					Debug.Log($"No clicked on entities");
					return;
				}

				//Otherwise, we have some hits. Let's check them.
				//TODO: This is kind of slow.
				GameObject rootGameObject = CachedHitResults[0].transform.GetRootGameObject();

				if(GameObjectToEntityMappable.ObjectToEntityMap.ContainsKey(rootGameObject))
				{
					NetworkEntityGuid entity = GameObjectToEntityMappable.ObjectToEntityMap[rootGameObject];

					//If we actually have interacted with an entity that is on the UI layer
					//then we need to dispatch the event.
					Debug.Log($"Clicked on Entity: {entity.EntityType}:{entity.EntityId}");
				}
				else
					Debug.Log($"No entity. Clicked on: {rootGameObject.name}");
			}
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			CameraReference = Camera.main;
			return Task.CompletedTask;
		}
	}
}
