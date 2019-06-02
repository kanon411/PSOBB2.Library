﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
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
					//TODO: Hack to support VR builds with in-editor non-VR.
					if(Application.isEditor)
					{
						//TODO: We should handle prefabs better
						return Resources.Load<GameObject>("Prefabs/LocalPlayerAvatar");
					}
					else
					{
						//TODO: Renable VR builds someday
						//return Resources.Load<GameObject>("Prefabs/LocalPlayerAvatar_vr");
						return Resources.Load<GameObject>("Prefabs/LocalPlayerAvatar");
					}
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
