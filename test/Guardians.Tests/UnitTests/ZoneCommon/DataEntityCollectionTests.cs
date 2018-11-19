using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Guardians.Tests.Collections
{
	[TestFixture]
	public sealed class DataEntityCollectionTests
	{
		[Test]
		public void Test_Can_Create_Ctor()
		{
			Assert.DoesNotThrow(() => new EntityFieldDataCollection<TestFieldType>());
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

		private static void GenericInitialValueTest<TValueType>(int index) 
			where TValueType : struct
		{
			//arrange
			EntityFieldDataCollection<TestFieldType> collection = new EntityFieldDataCollection<TestFieldType>();

			//act
			TValueType value = collection.GetFieldValue<TValueType>(index);

			//assert
			Assert.AreEqual(default(TValueType), value, $"Value at Index: {index} is suppose to be zero or default value: {default(TValueType)}.");
		}
	}
}
