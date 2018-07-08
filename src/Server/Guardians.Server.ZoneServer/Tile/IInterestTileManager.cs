﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IInterestTileManager : IEntityGateway<int>, IRegisterable<int, object> //TODO: Have register details
	{
		/// <summary>
		/// The tiles, mapped by tile id, in the tile manager.
		/// </summary>
		IReadOnlyDictionary<int, IReadonlyInterestTile> Tiles { get; }
	}
}
