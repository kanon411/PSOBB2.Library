using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	/// <summary>
	/// Enumeration of entity prefab IDs.
	/// (This is not models/meshes but the actualt prefab objects themselves).
	/// </summary>
	public enum EntityPrefab : int
	{
		Unknown = 0,
		LocalPlayer = 1,
		RemotePlayer = 2,

		NetworkNpc = 3
	}
}
