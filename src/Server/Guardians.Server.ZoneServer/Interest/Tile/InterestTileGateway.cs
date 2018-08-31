using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using JetBrains.Annotations;
using PostSharp.Patterns.Caching;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Component that acts as a gateway for interest tiles.
	/// Entites can enter it and leave it.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public sealed class InterestTileGateway : MonoBehaviour, IPhysicsTriggerCallbackable
	{
		/// <summary>
		/// The ID of the interest tile.
		/// </summary>
		public int TileGatewayId => ComputeTileId();

		[Inject]
		private IInterestTileManager TileManager { get; }

		[Inject]
		private ILog Logger { get; }

		[Inject]
		private IReadonlyGameObjectToEntityMappable ObjectToEntityMapper { get; }

		[Cache]
		private int ComputeTileId()
		{
			//TODO: Don't hardcode the size.
			//We shift the x index to the upper 4 bytes
			return (((int)transform.position.x / 10) << 16) + ((int)transform.position.z / 10);
		}

		/// <inheritdoc />
		public void OnTriggerEnter([NotNull] Collider other)
		{
			if(other == null) throw new ArgumentNullException(nameof(other));

			GameObject rootObject = other.GetRootGameObject();

			if(!ObjectToEntityMapper.ObjectToEntityMap.ContainsKey(rootObject))
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Tried to enter Entity: {rootObject.name} from Tile ID: {TileGatewayId} but does not exist. Is not owned.");
				return;
			}

			bool result = TileManager.TryEntityEnter(TileGatewayId, ObjectToEntityMapper.ObjectToEntityMap[rootObject]);

			if(!result)
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to enter Entity: {ObjectToEntityMapper.ObjectToEntityMap[rootObject]} to Tile: {TileGatewayId}");
		}

		/// <inheritdoc />
		public void OnTriggerExit([NotNull] Collider other)
		{
			if(other == null) throw new ArgumentNullException(nameof(other));

			GameObject rootObject = other.GetRootGameObject();

			if(!ObjectToEntityMapper.ObjectToEntityMap.ContainsKey(rootObject))
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Tried to remove Entity: {rootObject.name} from Tile ID: {TileGatewayId} but does not exist. Is not owned.");
				return;
			}

			bool result = TileManager.TryEntityLeave(TileGatewayId, ObjectToEntityMapper.ObjectToEntityMap[rootObject]);

			if(!result)
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to exit Entity: {ObjectToEntityMapper.ObjectToEntityMap[rootObject]} to Tile: {TileGatewayId}");
		}
	}
}
