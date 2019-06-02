﻿namespace PSOBB
{
	/// <summary>
	/// Enumeration of all response codes associated with <see cref="ZoneServerNPCEntryCollectionResponse"/>.
	/// </summary>
	public enum NpcEntryCollectionResponseCode
	{
		Success = 1,

		/// <summary>
		/// Indicates that no entries were found.
		/// </summary>
		NoneFound = 2,

		//TODO: Implement potential error codes
	}
}