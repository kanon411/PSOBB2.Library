using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Enumeration of all the game zone types.
	/// </summary>
	[Flags]
	public enum GameZoneType : int
	{
		Unknown = 0,

		//These are suppose to be transcient zones like dungeons or battlegrounds
		Transient = 1,

		//TODO: Add actual static zones.
		ZoneFirst = 2,

		ZoneSecond = 3
	}
}
