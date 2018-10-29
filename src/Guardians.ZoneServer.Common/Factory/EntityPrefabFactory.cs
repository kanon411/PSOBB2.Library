using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	//TODO: This might not be something that should be common between client/server
	public sealed class EntityPrefabFactory : IFactoryCreatable<GameObject, EntityPrefab>
	{
		/// <inheritdoc />
		public GameObject Create(EntityPrefab context)
		{
			switch(context)
			{
				case EntityPrefab.Unknown:
					break;
				case EntityPrefab.LocalPlayer:
					//TODO: We should handle prefabs better
					return Resources.Load<GameObject>("Prefabs/LocalPlayerAvatar");
				case EntityPrefab.RemotePlayer:
					//TODO: We should handle prefabs better
					return Resources.Load<GameObject>("Prefabs/RemotePlayerAvatar");
				case EntityPrefab.NetworkNpc:
					return Resources.Load<GameObject>("Prefabs/NetworkNpc");
			}

			throw new NotImplementedException($"Failed to load prefab for {nameof(EntityPrefab)}: {context}");
		}
	}
}
