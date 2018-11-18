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
		public void Test_Initial_Index_Is_Zero([Range(0, 5)] int index)
		{
			GenericInitialValueTest<int>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Int64([Range(0, 5)] int index)
		{
			GenericInitialValueTest<Int64>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_UInt64([Range(0, 5)] int index)
		{
			GenericInitialValueTest<UInt64>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Short([Range(0, 5)] int index)
		{
			GenericInitialValueTest<short>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_UShort([Range(0, 5)] int index)
		{
			GenericInitialValueTest<ushort>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Byte([Range(0, 5)] int index)
		{
			GenericInitialValueTest<byte>(index);
		}

		[Test]
		public void Test_Initial_Index_Is_Zero_As_Float([Range(0, 5)] int index)
		{
			GenericInitialValueTest<float>(index);
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

		public enum TestFieldType
		{
			Value1 = 0,
			Value2 = 1,
			Value3 = 2,
			Value4 = 3,
			Value5 = 4,
			Value6 = 5
		}
	}
}
