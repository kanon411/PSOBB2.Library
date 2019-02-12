using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Non-generic version.
	/// </summary>
	public sealed class ChangeTrackingEntityFieldDataCollectionDecorator : IEntityDataFieldContainer, IChangeTrackableEntityDataCollection
	{
		/// <summary>
		/// The collection that tracks the dirty changes in values.
		/// </summary>
		public WireReadyBitArray ChangeTrackingArray { get; }

		//Just forward to decorated
		/// <inheritdoc />
		public WireReadyBitArray DataSetIndicationArray => EntityDataCollection.DataSetIndicationArray;

		/// <inheritdoc />
		public bool HasPendingChanges { get; private set; } = false;

		/// <inheritdoc />
		public void ClearTrackedChanges()
		{
			//TODO: This is slow and inefficient. We should maintain and memcpy an array of false bits. We may have to do this hundreds/thousands of a minute.
			ChangeTrackingArray.SetAll(false);
			HasPendingChanges = false;
		}

		/// <summary>
		/// The decorated entity data container.
		/// </summary>
		private IEntityDataFieldContainer EntityDataCollection { get; }

		/// <inheritdoc />
		public ChangeTrackingEntityFieldDataCollectionDecorator(IEntityDataFieldContainer entityDataCollection)
		{
			EntityDataCollection = entityDataCollection ?? throw new ArgumentNullException(nameof(entityDataCollection));
			ChangeTrackingArray = new WireReadyBitArray(entityDataCollection.DataSetIndicationArray.Length); //just the size of the initial data indiciation bitarray
		}

		/// <inheritdoc />
		public TValueType GetFieldValue<TValueType>(int index)
			where TValueType : struct
		{
			return EntityDataCollection.GetFieldValue<TValueType>(index);
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

				//We only have pending changes if the value is not equal
				HasPendingChanges = true;
			}
			else
			{
				//TODO: This kinda exposing an implementation detail because if we started with 0 and setting 0 the above if will fail.
				//The reasoning is if we explictly set 0 then we set the bit because it might not be set
				DataSetIndicationArray.Set(index, true);
			}
		}
	}

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

		//Just forward to decorated
		/// <inheritdoc />
		public WireReadyBitArray DataSetIndicationArray => EntityDataCollection.DataSetIndicationArray;

		/// <inheritdoc />
		public bool HasPendingChanges { get; private set; } = false;

		/// <inheritdoc />
		public void ClearTrackedChanges()
		{
			//TODO: This is slow and inefficient. We should maintain and memcpy an array of false bits. We may have to do this hundreds/thousands of a minute.
			ChangeTrackingArray.SetAll(false);
			HasPendingChanges = false;
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

				//We only have pending changes if the value is not equal
				HasPendingChanges = true;
			}
			else
			{
				//TODO: This kinda exposing an implementation detail because if we started with 0 and setting 0 the above if will fail.
				//The reasoning is if we explictly set 0 then we set the bit because it might not be set
				DataSetIndicationArray.Set(index, true);
			}
	}

		private static int ComputeDataFieldCollectionLength()
		{
			return Enum.GetValues(typeof(TFieldType)).Length;
		}
	}
}
