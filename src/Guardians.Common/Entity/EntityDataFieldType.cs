using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Enumeration of Entity Data FieldTypes.
	/// Each entry in this enumeration represents a 4 byte field
	/// of entity data.
	/// </summary>
	public enum EntityDataFieldType
	{
		/// <summary>
		/// The maximum health of the entity.
		/// Can be modified or changed, with bufs and etc.
		/// </summary>
		EntityMaxHealth = 0,

		/// <summary>
		/// The current health of the entity.
		/// </summary>
		EntityCurrentHealth = 1
	}
}
