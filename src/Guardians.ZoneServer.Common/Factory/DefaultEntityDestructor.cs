using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class DefaultEntityDestructor : IObjectDestructorable<NetworkEntityGuid>
	{
		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<IMovementData> GuidToMovementInfoMappable { get; }

		private IGameObjectToEntityMappable GameObjectToEntityMap { get; }

		/// <inheritdoc />
		public DefaultEntityDestructor(
			IEntityGuidMappable<GameObject> guidToGameObjectMappable, 
			IEntityGuidMappable<IMovementData> guidToMovementInfoMappable, 
			IGameObjectToEntityMappable gameObjectToEntityMap)
		{
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
			GameObjectToEntityMap = gameObjectToEntityMap ?? throw new ArgumentNullException(nameof(gameObjectToEntityMap));
		}

		/// <inheritdoc />
		public bool Destroy(NetworkEntityGuid obj)
		{
			//TODO: Verify we even know it.
			//TODO: We may not want to do this for group members?
			//We destroy an entity via its entity guid, just remove all references.

			GameObject rootEntityGameObject = GuidToGameObjectMappable[obj];

			GuidToMovementInfoMappable.Remove(obj);
			GuidToGameObjectMappable.Remove(obj);
			GameObjectToEntityMap.ObjectToEntityMap.Remove(rootEntityGameObject);

			GameObject.Destroy(rootEntityGameObject);

			return true;
		}
	}
}
