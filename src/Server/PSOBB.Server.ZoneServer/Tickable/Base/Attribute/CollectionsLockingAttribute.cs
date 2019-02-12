using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PSOBB
{
	//TODO: Move somewhere else?
	/// <summary>
	/// Enumeration of the types of locking.
	/// </summary>
	public enum LockType
	{
		Read = 1,

		Write = 2,
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class CollectionsLockingAttribute : Attribute
	{
		/// <summary>
		/// The locking type to apply to the collections
		/// lock.
		/// </summary>
		public LockType DesiredLockingType { get; }

		/// <inheritdoc />
		public CollectionsLockingAttribute(LockType desiredLockingType)
		{
			if(!Enum.IsDefined(typeof(LockType), desiredLockingType)) throw new InvalidEnumArgumentException(nameof(desiredLockingType), (int)desiredLockingType, typeof(LockType));
			DesiredLockingType = desiredLockingType;
		}
	}
}
