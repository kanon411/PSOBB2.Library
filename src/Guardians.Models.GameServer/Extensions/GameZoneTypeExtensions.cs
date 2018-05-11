using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public static class GameZoneTypeExtensions
	{
		/// <summary>
		/// Indicates if the zone type is static.
		/// </summary>
		/// <param name="type">The zone type.</param>
		/// <returns>True if the zone type is static.</returns>
		public static bool IsStatic(this GameZoneType type)
		{
			return type != GameZoneType.Transient && type.IsKnown();
		}

		/// <summary>
		/// Indicates if this is a known zone.
		/// </summary>
		/// <param name="type">The zone type.</param>
		/// <returns>True if the zone type is a known type.</returns>
		public static bool IsKnown(this GameZoneType type)
		{
			return type != GameZoneType.Unknown;
		}
	}
}
