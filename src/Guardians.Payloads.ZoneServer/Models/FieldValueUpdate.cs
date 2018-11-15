using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Model that represents an updated field values.
	/// Using the concept of Dirty/Update values instead
	/// of the entire field value collection.
	/// </summary>
	[ProtoContract]
	public class FieldValueUpdate
	{
		/// <summary>
		/// The update bit mask that indicates
		/// what values are being sent/updates.
		/// </summary>
		[ProtoMember(1)]
		public WireReadyBitArray FieldValueUpdateMask { get; private set; }

		/// <summary>
		/// The updated field values.
		/// See the set bits in <see cref="FieldValueUpdateMask"/> to
		/// known what these values are.
		/// </summary>
		[ProtoMember(1, IsPacked = true)]
		public int[] FieldValueUpdates { get; private set; }

		/// <inheritdoc />
		public FieldValueUpdate([NotNull] WireReadyBitArray fieldValueUpdateMask, [NotNull] int[] fieldValueUpdates)
		{
			//We shouldn't send an a field value update if there are no fields to even update. It's pointless bandwidth wasted
			if(fieldValueUpdates == null) throw new ArgumentNullException(nameof(fieldValueUpdates));
			if(fieldValueUpdates.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(fieldValueUpdates));

			FieldValueUpdateMask = fieldValueUpdateMask ?? throw new ArgumentNullException(nameof(fieldValueUpdateMask));
			FieldValueUpdates = fieldValueUpdates;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private FieldValueUpdate()
		{
			
		}
	}
}
