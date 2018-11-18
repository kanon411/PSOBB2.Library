using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Guardians
{
	public sealed class ChangeTrackingEntityFieldDataCollectionDecorator<TFieldType> : IEntityDataFieldContainer<TFieldType>, IChangeTrackableEntityDataCollection
		where TFieldType : struct //TODO: When C# 8.0 or 7.3 is better supported switch to it for Enum constraint
	{
		//TODO: This is a hack until Enum constraint.
		static ChangeTrackingEntityFieldDataCollectionDecorator()
		{
			if(!typeof(TFieldType).IsEnum)
				throw new InvalidOperationException($"Type: {typeof(TFieldType).Name} {nameof(TFieldType)} MUST be an enum type for Type: {typeof(ChangeTrackingEntityFieldDataCollectionDecorator<>)}");
		}

		/// <summary>
		/// The collection that tracks the dirty changes in values.
		/// </summary>
		public WireReadyBitArray ChangeTrackingArray { get; }

		/// <inheritdoc />
		public void ClearTrackedChanges()
		{
			//TODO: This is slow and inefficient. We should maintain and memcpy an array of false bits. We may have to do this hundreds/thousands of a minute.
			ChangeTrackingArray.SetAll(false);
		}

		/// <summary>
		/// The decorated entity data container.
		/// </summary>
		private IEntityDataFieldContainer<TFieldType> EntityDataCollection { get; }

		/// <inheritdoc />
		public ChangeTrackingEntityFieldDataCollectionDecorator(IEntityDataFieldContainer<TFieldType> entityDataCollection)
		{
			EntityDataCollection = entityDataCollection ?? throw new ArgumentNullException(nameof(entityDataCollection));
			ChangeTrackingArray = new WireReadyBitArray(ComputeDataFieldCollectionLength());
		}

		/// <inheritdoc />
		public TValueType GetFieldValue<TValueType>(TFieldType index) 
			where TValueType : struct
		{
			return EntityDataCollection.GetFieldValue<TValueType>(index);
		}

		/// <inheritdoc />
		public TValueType GetFieldValue<TValueType>(int index) 
			where TValueType : struct
		{
			return EntityDataCollection.GetFieldValue<TValueType>(index);
		}

		/// <inheritdoc />
		public void SetFieldValue<TValueType>(TFieldType index, TValueType value) 
			where TValueType : struct
		{
			int indexAsInt = Unsafe.As<TFieldType, int>(ref index);

			this.SetFieldValue(indexAsInt, value);
		}

		/// <inheritdoc />
		public void SetFieldValue<TValueType>(int index, TValueType value) 
			where TValueType : struct
		{
			int potentialNewValue = Unsafe.As<TValueType, int>(ref value);

			//If the values aren't equal we need to set the tracking/dirty stuff
			//Then we also should set the data
			if(!potentialNewValue.Equals(EntityDataCollection.GetFieldValue<int>(index)))
			{
				ChangeTrackingArray.Set(index, true);
				EntityDataCollection.SetFieldValue(index, value);
			}
		}

		private static int ComputeDataFieldCollectionLength()
		{
			return Enum.GetValues(typeof(TFieldType)).Length;
		}
	}
}
