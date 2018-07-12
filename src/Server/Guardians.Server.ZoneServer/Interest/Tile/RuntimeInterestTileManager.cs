using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class RuntimeInterestTileManager : IInterestTileManager, IGameTickable
	{
		private InterestTileDictionary _Tiles { get; }

		/// <inheritdoc />
		public IReadOnlyDictionary<int, IReadonlyInterestCollection> Tiles => _Tiles;

		/// <summary>
		/// The tile logger.
		/// </summary>
		private ILog Logger { get; }

		/// <inheritdoc />
		public RuntimeInterestTileManager([NotNull] InterestTileDictionary tiles, [NotNull] ILog logger)
		{
			_Tiles = tiles ?? throw new ArgumentNullException(nameof(tiles));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public RuntimeInterestTileManager(ILog logger)
			: this(new InterestTileDictionary(), logger)
		{

		}

		public void BuildTileGroups()
		{
			//Tile groups encompass 9 tiles. A middle tile, the main tile, and neighboring tiles
			//these tiles are shared throughtout other groups too.
		}

		/// <inheritdoc />
		public bool TryEntityEnter(int entryContext, NetworkEntityGuid entityGuid)
		{
			ThrowIfTileDoesNotExist(entryContext, entityGuid);

			_Tiles[entryContext].Register(entityGuid, entityGuid);

			return true;
		}

		private void ThrowIfTileDoesNotExist(int entryContext, NetworkEntityGuid entityGuid)
		{
			if(!Contains(entryContext))
				throw new KeyNotFoundException($"Tile with Id: {entryContext} not found in the {GetType().Name} for Entity ID: {entityGuid}");
		}

		/// <inheritdoc />
		public bool TryEntityLeave(int entryContext, NetworkEntityGuid entityGuid)
		{
			ThrowIfTileDoesNotExist(entryContext, entityGuid);

			return _Tiles[entryContext].Unregister(entityGuid);
		}

		/// <inheritdoc />
		public void Register(int key, object value)
		{
			_Tiles.Add(key, new InterestTile(key));

			if(Logger.IsInfoEnabled)
				Logger.Info($"Registered tile with Id: {key}");
		}

		/// <inheritdoc />
		public bool Contains(int key)
		{
			return _Tiles.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool Unregister(int key)
		{
			bool result = _Tiles.Remove(key);

			if(result && Logger.IsInfoEnabled)
				Logger.Info($"Unregistered tile with Id: {key}");
			else if(Logger.IsErrorEnabled)
				Logger.Error($"Failed to unregister tile with Id: {key}");

			return result;
		}

		/// <inheritdoc />
		object IRegisterable<int, object>.Retrieve(int key)
		{
			throw new NotSupportedException($"Cannot retrieve tiles.");
		}

		/// <inheritdoc />
		public void Tick()
		{

		}
	}
}
