﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace PSOBB.Tests.Collections
{
	[TestFixture]
	public class DataEntityCollectionTests
	{
		[Test]
		public void Test_Can_Create_Ctor()
		{
			Assert.DoesNotThrow(() => CreateEntityDataCollection());
		}

		[Test]
		public void Test_Initial_Index_Is_Zero([EntityDataCollectionTestRange] int index)
		{
			GenericInitialValueTest<int>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Int64([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<Int64>(index));
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_UInt64([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<UInt64>(index));
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Short([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<short>(index));
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_UShort([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<ushort>(index));
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Byte([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<byte>(index));
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Float([EntityDataCollectionTestRange] int index)
		{
			Assert.DoesNotThrow(() => GenericInitialValueTest<float>(index));
		}

		[Test]
		public void Test_Value_Set_Same_Read_Value([EntityDataCollectionTestRange] int index, [Values(1, 2, 3, 4, 5, 6, 7, 8)] int value)
		{
			//arrange
			IEntityDataFieldContainer<TestFieldType> collection = CreateEntityDataCollection();

			//act
			collection.SetFieldValue<int>(index, value);
			int getValue = collection.GetFieldValue<int>(index);

			//assert
			Assert.AreEqual(value, getValue, $"Set Value: {value} at Index: {index} was wrong Value: {getValue} instead.");
		}

		[Test]
		public void Test_DataIndexSetTracking_Indicates_Set_Indicies([EntityDataCollectionTestRange] int index1, [EntityDataCollectionTestRange] int index2, [Values(-1, 0, 1, 2)] int value1, [Values(-1, 0, 1, 2)] int value2)
		{
			//arrange
			IEntityDataFieldContainer<TestFieldType> collection = CreateEntityDataCollection();

			//act
			collection.SetFieldValue<int>(index1, value1);
			collection.SetFieldValue<int>(index2, value2);
			bool isSet1 = collection.DataSetIndicationArray.Get(index1);
			bool isSet2 = collection.DataSetIndicationArray.Get(index2);

			//assert
			Assert.True(isSet1, $"Index: {index1} being set should have caused indication array to be set.");
			Assert.True(isSet2, $"Index: {index2} being set should have caused indication array to be set.");
		}

		[Test]
		public void Test_Value_Set_Multiple_Times_Is_Latest_Value([EntityDataCollectionTestRange] int index, [Values(1, 2, 3, 4, 5, 6, 7, 8)] int value)
		{
			//arrange
			IEntityDataFieldContainer<TestFieldType> collection = CreateEntityDataCollection();

			//act
			collection.SetFieldValue<int>(index, 5000); //test set to 5000, will check the resulting end value
			collection.SetFieldValue<int>(index, value);
			int getValue = collection.GetFieldValue<int>(index);

			//assert
			Assert.AreEqual(value, getValue, $"Set Value: {value} at Index: {index} was wrong Value: {getValue} instead.");
		}

		protected virtual IEntityDataFieldContainer<TestFieldType> CreateEntityDataCollection()
		{
			return new EntityFieldDataCollection<TestFieldType>();
		}

		private void GenericInitialValueTest<TValueType>(int index) 
			where TValueType : struct
		{
			//arrange
			IEntityDataFieldContainer<TestFieldType> collection = CreateEntityDataCollection();

			//act
			TValueType value = collection.GetFieldValue<TValueType>(index);

			//assert
			Assert.AreEqual(default(TValueType), value, $"Value at Index: {index} is suppose to be zero or default value: {default(TValueType)}.");
		}
	}
}
