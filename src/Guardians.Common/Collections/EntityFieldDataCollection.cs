using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Generic.Math;

namespace Guardians
{
	public sealed class EntityFieldDataCollection<TFieldType> : IReadonlyEntityDataFieldContainer, IReadonlyEntityDataFieldContainer<TFieldType>
		where TFieldType : struct //TODO: When C# 8.0 or 7.3 is better supported switch to it for Enum constraint
	{
		//TODO: This is a hack until Enum constraint.
		static EntityFieldDataCollection()
		{
			if(!typeof(TFieldType).IsEnum)
				throw new InvalidOperationException($"Type: {typeof(TFieldType).Name} {nameof(TFieldType)} MUST be an enum type for Type: {typeof(EntityFieldDataCollection<>)}");
		}

		//Data fields are modeled as 4 byte fields.
		//.NET runtime does a really good job of optimizing Int32 operations
		//and Int32 arrays. Most fields are small enough to be represented by integer fields
		//and the largest 64bit fields can just take up 2 slots.
		private int[] InternalDataFields { get; }

		public EntityFieldDataCollection()
		{
			//TODO: Make this allocation more efficient. Maybe even use pooling.
			InternalDataFields = new int[ComputeDataFieldCollectionLength()];
		}

		private static int ComputeDataFieldCollectionLength()
		{
			return Enum.GetValues(typeof(TFieldType)).Length;
		}

		/// <inheritdoc />
		public EntityFieldDataCollection(int[] internalDataFields)
		{
			if(internalDataFields == null) throw new ArgumentNullException(nameof(internalDataFields));
			if(internalDataFields.Length != ComputeDataFieldCollectionLength())
				throw new InvalidOperationException($"Cannot initialize: {GetType().Name} with data fields Length: {internalDataFields}. Required exact Length: {ComputeDataFieldCollectionLength()}");

			InternalDataFields = internalDataFields;
		}

		//TFieldType
		public TValueType GetFieldValue<TValueType>(TFieldType index)
			where TValueType : struct
		{
			return GetFieldValue<TValueType>(GenericMath.Convert<TFieldType, int>(index));
		}

		//TODO: Would ref return be better here? Maybe only for 64bits?
		public TValueType GetFieldValue<TValueType>(int index)
			where TValueType : struct
		{
			if(index >= InternalDataFields.Length)
				throw new ArgumentOutOfRangeException(nameof(index), $"Provided Index: {index} was out of range. Max index for Type: {typeof(TFieldType).Name} is Index: {InternalDataFields.Length - 1}");

			//Just assume we can do it, the caller is responsible for the diaster.
			return Unsafe.As<int, TValueType>(ref InternalDataFields[index]);
		}
	}
}
