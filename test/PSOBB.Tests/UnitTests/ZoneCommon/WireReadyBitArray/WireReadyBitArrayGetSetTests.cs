using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PSOBB.Tests.Collections
{
	public static class WireReadyBitArrayGetSetTests
	{
		private const int BitsPerByte = 8;
		private const int BitsPerInt32 = 32;

		public static IEnumerable<object[]> Get_Set_Data()
		{
			foreach(int size in new[] { 0, 1, BitsPerByte, BitsPerByte * 2, BitsPerInt32, BitsPerInt32 * 2 })
			{
				foreach(bool def in new[] { true, false })
				{
					yield return new object[] { def, Enumerable.Repeat(true, size).ToArray() };
					yield return new object[] { def, Enumerable.Repeat(false, size).ToArray() };
					yield return new object[] { def, Enumerable.Range(0, size).Select(i => i % 2 == 1).ToArray() };
				}
			}
		}

		[Test]
		[TestCaseSource(nameof(Get_Set_Data))]
		public static void Get_Set(bool def, bool[] newValues)
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(newValues.Length, def);
			for(int i = 0; i < newValues.Length; i++)
			{
				WireReadyBitArray.Set(i, newValues[i]);
				Assert.AreEqual(newValues[i], WireReadyBitArray[i]);
				Assert.AreEqual(newValues[i], WireReadyBitArray.Get(i));
			}
		}

		[Test]
		public static void Get_InvalidIndex_ThrowsArgumentOutOfRangeException()
		{
			WireReadyBitArray bitArray = new WireReadyBitArray(4);
			Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.Get(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.Get(bitArray.Length));

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				var l = bitArray[-1];
			});
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				var l = bitArray[bitArray.Length];
			});
		}

		[Test]
		public static void Set_InvalidIndex_ThrowsArgumentOutOfRangeException()
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(4);
			Assert.Throws<ArgumentOutOfRangeException>(() => WireReadyBitArray.Set(-1, true));
			Assert.Throws<ArgumentOutOfRangeException>(() => WireReadyBitArray.Set(WireReadyBitArray.Length, true));

			Assert.Throws<ArgumentOutOfRangeException>(() => WireReadyBitArray[-1] = true);
			Assert.Throws<ArgumentOutOfRangeException>(() => WireReadyBitArray[WireReadyBitArray.Length] = true);
		}

		[Test]
		[TestCase(0, true)]
		[TestCase(0, false)]
		[TestCase(1, true)]
		[TestCase(1, false)]
		[TestCase(BitsPerByte, true)]
		[TestCase(BitsPerByte, false)]
		[TestCase(BitsPerByte + 1, true)]
		[TestCase(BitsPerByte + 1, false)]
		[TestCase(BitsPerInt32, true)]
		[TestCase(BitsPerInt32, false)]
		[TestCase(BitsPerInt32 + 1, true)]
		[TestCase(BitsPerInt32 + 1, false)]
		public static void SetAll(int size, bool defaultValue)
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(size, defaultValue);
			WireReadyBitArray.SetAll(!defaultValue);
			for(int i = 0; i < WireReadyBitArray.Length; i++)
			{
				Assert.AreEqual(!defaultValue, WireReadyBitArray[i]);
				Assert.AreEqual(!defaultValue, WireReadyBitArray.Get(i));
			}

			WireReadyBitArray.SetAll(defaultValue);
			for(int i = 0; i < WireReadyBitArray.Length; i++)
			{
				Assert.AreEqual(defaultValue, WireReadyBitArray[i]);
				Assert.AreEqual(defaultValue, WireReadyBitArray.Get(i));
			}
		}

		public static IEnumerable<object[]> GetEnumerator_Data()
		{
			foreach(int size in new[] { 0, 1, BitsPerByte, BitsPerByte + 1, BitsPerInt32, BitsPerInt32 + 1 })
			{
				foreach(bool lead in new[] { true, false })
				{
					yield return new object[] { Enumerable.Range(0, size).Select(i => lead ^ (i % 2 == 0)).ToArray() };
				}
			}
		}

		/*[Test]
		[TestCaseSource(nameof(GetEnumerator_Data))]
		public static void GetEnumerator(bool[] values)
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(values);
			Assert.AreNotSame(WireReadyBitArray.GetEnumerator(), WireReadyBitArray.GetEnumerator());
			IEnumerator enumerator = WireReadyBitArray.GetEnumerator();
			for(int i = 0; i < 2; i++)
			{
				int counter = 0;
				while(enumerator.MoveNext())
				{
					Assert.AreEqual(WireReadyBitArray[counter], enumerator.Current);
					counter++;
				}
				Assert.AreEqual(WireReadyBitArray.Length, counter);
				enumerator.Reset();
			}
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(BitsPerByte)]
		[TestCase(BitsPerByte + 1)]
		[TestCase(BitsPerInt32)]
		[TestCase(BitsPerInt32 + 1)]
		public static void GetEnumerator_Invalid(int size)
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(size, true);
			IEnumerator enumerator = WireReadyBitArray.GetEnumerator();

			// Has not started enumerating
			Assert.Throws<InvalidOperationException>(() => enumerator.Current);

			// Has finished enumerating
			while(enumerator.MoveNext()) ;
			Assert.Throws<InvalidOperationException>(() => enumerator.Current);

			// Has resetted enumerating
			enumerator.Reset();
			Assert.Throws<InvalidOperationException>(() => enumerator.Current);

			// Has modified underlying collection
			if(size > 0)
			{
				enumerator.MoveNext();
				WireReadyBitArray[0] = false;
				Assert.True((bool)enumerator.Current);
				Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
				Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
			}
		}

		[Test]
		public static void GetEnumerator_CloneEnumerator_ReturnsUniqueEnumerator()
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(1);
			IEnumerator enumerator = WireReadyBitArray.GetEnumerator();
			ICloneable cloneableEnumerator = enumerator as ICloneable;
			Assert.NotNull(cloneableEnumerator);

			IEnumerator clonedEnumerator = (IEnumerator)cloneableEnumerator.Clone();
			Assert.AreNotSame(enumerator, clonedEnumerator);

			Assert.True(clonedEnumerator.MoveNext());
			Assert.False(clonedEnumerator.MoveNext());

			Assert.True(enumerator.MoveNext());
			Assert.False(enumerator.MoveNext());
		}*/

		public static IEnumerable<object[]> Length_Set_Data()
		{
			int[] sizes = { 1, BitsPerByte, BitsPerByte + 1, BitsPerInt32, BitsPerInt32 + 1 };
			foreach(int original in sizes.Concat(new[] { 16384 }))
			{
				foreach(int n in sizes)
				{
					yield return new object[] { original, n };
				}
			}
		}

		/*[Test]
		[TestCaseSource(nameof(Length_Set_Data))]
		public static void Length_Set(int originalSize, int newSize)
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(originalSize, true);
			WireReadyBitArray.Length = newSize;
			Assert.AreEqual(newSize, WireReadyBitArray.Length);
			for(int i = 0; i < Math.Min(originalSize, WireReadyBitArray.Length); i++)
			{
				Assert.True(WireReadyBitArray[i]);
				Assert.True(WireReadyBitArray.Get(i));
			}
			for(int i = originalSize; i < newSize; i++)
			{
				Assert.False(WireReadyBitArray[i]);
				Assert.False(WireReadyBitArray.Get(i));
			}
			Assert.Throws<ArgumentOutOfRangeException>("index", () => WireReadyBitArray[newSize]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => WireReadyBitArray.Get(newSize));

			// Decrease then increase size
			WireReadyBitArray.Length = 0;
			Assert.AreEqual(0, WireReadyBitArray.Length);

			WireReadyBitArray.Length = newSize;
			Assert.AreEqual(newSize, WireReadyBitArray.Length);
			Assert.False(WireReadyBitArray.Get(0));
			Assert.False(WireReadyBitArray.Get(newSize - 1));
		}

		[Test]
		public static void Length_Set_InvalidLength_ThrowsArgumentOutOfRangeException()
		{
			WireReadyBitArray WireReadyBitArray = new WireReadyBitArray(1);
			Assert.Throws<ArgumentOutOfRangeException>(() => WireReadyBitArray.Length = -1);
		}

		public static IEnumerable<object[]> CopyTo_Array_TestData()
		{
			yield return new object[] { new WireReadyBitArray(0), 0, 0, new bool[0], default(bool) };
			yield return new object[] { new WireReadyBitArray(0), 0, 0, new byte[0], default(byte) };
			yield return new object[] { new WireReadyBitArray(0), 0, 0, new int[0], default(int) };

			foreach(int WireReadyBitArraySize in new[] { 0, 1, BitsPerByte, BitsPerByte * 2, BitsPerInt32, BitsPerInt32 * 2 })
			{
				WireReadyBitArray allTrue = new WireReadyBitArray(Enumerable.Repeat(true, WireReadyBitArraySize).ToArray());
				WireReadyBitArray allFalse = new WireReadyBitArray(Enumerable.Repeat(false, WireReadyBitArraySize).ToArray());
				WireReadyBitArray alternating = new WireReadyBitArray(Enumerable.Range(0, WireReadyBitArraySize).Select(i => i % 2 == 1).ToArray());

				foreach(var d in new[] { Tuple.Create(WireReadyBitArraySize, 0),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, 0),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, WireReadyBitArraySize + 1),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, WireReadyBitArraySize / 2 + 1) })
				{
					int arraySize = d.Item1;
					int index = d.Item2;

					yield return new object[] { allTrue, arraySize, index, Enumerable.Repeat(true, WireReadyBitArraySize).ToArray(), default(bool) };
					yield return new object[] { allFalse, arraySize, index, Enumerable.Repeat(false, WireReadyBitArraySize).ToArray(), default(bool) };
					yield return new object[] { alternating, arraySize, index, Enumerable.Range(0, WireReadyBitArraySize).Select(i => i % 2 == 1).ToArray(), default(bool) };

					if(WireReadyBitArraySize >= BitsPerByte)
					{
						yield return new object[] { allTrue, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0xff, WireReadyBitArraySize / BitsPerByte).ToArray(), default(byte) };
						yield return new object[] { allFalse, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0x00, WireReadyBitArraySize / BitsPerByte).ToArray(), default(byte) };
						yield return new object[] { alternating, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0xaa, WireReadyBitArraySize / BitsPerByte).ToArray(), default(byte) };
					}

					if(WireReadyBitArraySize >= BitsPerInt32)
					{
						yield return new object[] { allTrue, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xffffffff), WireReadyBitArraySize / BitsPerInt32).ToArray(), default(int) };
						yield return new object[] { allFalse, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(0x00000000, WireReadyBitArraySize / BitsPerInt32).ToArray(), default(int) };
						yield return new object[] { alternating, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xaaaaaaaa), WireReadyBitArraySize / BitsPerInt32).ToArray(), default(int) };
					}
				}
			}

			foreach(int WireReadyBitArraySize in new[] { BitsPerInt32 - 1, BitsPerInt32 * 2 - 1 })
			{
				WireReadyBitArray allTrue = new WireReadyBitArray(Enumerable.Repeat(true, WireReadyBitArraySize).ToArray());
				WireReadyBitArray allFalse = new WireReadyBitArray(Enumerable.Repeat(false, WireReadyBitArraySize).ToArray());
				WireReadyBitArray alternating = new WireReadyBitArray(Enumerable.Range(0, WireReadyBitArraySize).Select(i => i % 2 == 1).ToArray());

				foreach(var d in new[] { Tuple.Create(WireReadyBitArraySize, 0),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, 0),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, WireReadyBitArraySize + 1),
					Tuple.Create(WireReadyBitArraySize * 2 + 1, WireReadyBitArraySize / 2 + 1)})
				{
					int arraySize = d.Item1;
					int index = d.Item2;

					if(WireReadyBitArraySize >= BitsPerInt32)
					{
						yield return new object[] { allTrue, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xffffffff), WireReadyBitArraySize / BitsPerInt32).Concat(new[] { unchecked((int)(0xffffffffu >> 1)) }).ToArray(), default(int) };
						yield return new object[] { allFalse, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(0x00000000, WireReadyBitArraySize / BitsPerInt32 + 1).ToArray(), default(int) };
						yield return new object[] { alternating, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xaaaaaaaa), WireReadyBitArraySize / BitsPerInt32).Concat(new[] { unchecked((int)(0xaaaaaaaau >> 2)) }).ToArray(), default(int) };
					}
				}
			}
		}

		[Test]
		[TestCaseSource(nameof(CopyTo_Array_TestData))]
		public static void CopyTo<T>(WireReadyBitArray WireReadyBitArray, int length, int index, T[] expected, T def)
		{
			T[] array = (T[])Array.CreateInstance(typeof(T), length);
			ICollection collection = WireReadyBitArray;
			collection.CopyTo(array, index);
			for(int i = 0; i < index; i++)
			{
				Assert.AreEqual(def, array[i]);
			}
			for(int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], array[i + index]);
			}
			for(int i = index + expected.Length; i < array.Length; i++)
			{
				Assert.AreEqual(def, array[i]);
			}
		}

		[Test]
		public static void CopyTo_Type_Invalid()
		{
			ICollection WireReadyBitArray = new WireReadyBitArray(10);
			Assert.Throws<ArgumentNullException>("array", () => WireReadyBitArray.CopyTo(null, 0));
			Assert.Throws<ArgumentException>("array", null, () => WireReadyBitArray.CopyTo(new long[10], 0));
			Assert.Throws<ArgumentException>("array", null, () => WireReadyBitArray.CopyTo(new int[10, 10], 0));
		}

		[Test]
		[TestCase(default(bool), 1, 0, 0)]
		[TestCase(default(bool), 1, 1, 1)]
		[TestCase(default(bool), BitsPerByte, BitsPerByte - 1, 0)]
		[TestCase(default(bool), BitsPerByte, BitsPerByte, 1)]
		[TestCase(default(bool), BitsPerInt32, BitsPerInt32 - 1, 0)]
		[TestCase(default(bool), BitsPerInt32, BitsPerInt32, 1)]
		[TestCase(default(byte), BitsPerByte, 0, 0)]
		[TestCase(default(byte), BitsPerByte, 1, 1)]
		[TestCase(default(byte), BitsPerByte * 4, 4 - 1, 0)]
		[TestCase(default(byte), BitsPerByte * 4, 4, 1)]
		[TestCase(default(int), BitsPerInt32, 0, 0)]
		[TestCase(default(int), BitsPerInt32, 1, 1)]
		[TestCase(default(int), BitsPerInt32 * 4, 4 - 1, 0)]
		[TestCase(default(int), BitsPerInt32 * 4, 4, 1)]
		public static void CopyTo_Size_Invalid<T>(T def, int bits, int arraySize, int index)
		{
			ICollection WireReadyBitArray = new WireReadyBitArray(bits);
			T[] array = (T[])Array.CreateInstance(typeof(T), arraySize);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => WireReadyBitArray.CopyTo(array, -1));
			if(def is int)
			{
				Assert.Throws<ArgumentException>("destinationArray", string.Empty, () => WireReadyBitArray.CopyTo(array, index));
			}
			else
			{
				Assert.Throws<ArgumentException>(null, () => WireReadyBitArray.CopyTo(array, index));
			}
		}

		[Test]
		public static void SyncRoot()
		{
			ICollection WireReadyBitArray = new WireReadyBitArray(10);
			Assert.Same(WireReadyBitArray.SyncRoot, WireReadyBitArray.SyncRoot);
			Assert.AreNotSame(WireReadyBitArray.SyncRoot, ((ICollection)new WireReadyBitArray(10)).SyncRoot);
		}

		public static IEnumerable<object> CopyTo_Hidden_Data()
		{
			yield return new object[] { "ZeroLength", new WireReadyBitArray(0) };
			yield return new object[] { "Constructor", new WireReadyBitArray(BitsPerInt32 / 2 - 3, true) };
			yield return new object[] { "Not", new WireReadyBitArray(BitsPerInt32 / 2 - 3, false).Not() };
			WireReadyBitArray setAll = new WireReadyBitArray(BitsPerInt32 / 2 - 3, false);
			setAll.SetAll(true);
			yield return new object[] { "SetAll", setAll };
			WireReadyBitArray lengthShort = new WireReadyBitArray(BitsPerInt32, true);
			lengthShort.Length = BitsPerInt32 / 2 - 3;
			yield return new object[] { "Length-Short", lengthShort };
			WireReadyBitArray lengthLong = new WireReadyBitArray(2 * BitsPerInt32, true);
			lengthLong.Length = BitsPerInt32 - 3;
			yield return new object[] { "Length-Long < 32", lengthLong };
			WireReadyBitArray lengthLong2 = new WireReadyBitArray(2 * BitsPerInt32, true);
			lengthLong2.Length = BitsPerInt32 + 3;
			yield return new object[] { "Length-Long > 32", lengthLong2 };
			// alligned test cases
			yield return new object[] { "Aligned-Constructor", new WireReadyBitArray(BitsPerInt32, true) };
			yield return new object[] { "Aligned-Not", new WireReadyBitArray(BitsPerInt32, false).Not() };
			WireReadyBitArray alignedSetAll = new WireReadyBitArray(BitsPerInt32, false);
			alignedSetAll.SetAll(true);
			yield return new object[] { "Aligned-SetAll", alignedSetAll };
			WireReadyBitArray alignedLengthLong = new WireReadyBitArray(2 * BitsPerInt32, true);
			yield return new object[] { "Aligned-Length-Long", alignedLengthLong };
		}

		[Test]
		[SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
		[TestCaseSource(nameof(CopyTo_Hidden_Data))]
		public static void CopyTo_Int_Hidden(string label, WireReadyBitArray bits)
		{
			int allBitsSet = unchecked((int)0xffffffff); // 32 bits set to 1 = -1
			int fullInts = bits.Length / BitsPerInt32;
			int remainder = bits.Length % BitsPerInt32;
			int arrayLength = fullInts + (remainder > 0 ? 1 : 0);

			int[] data = new int[arrayLength];
			((ICollection)bits).CopyTo(data, 0);

			Assert.All(data.Take(fullInts), d => Assert.AreEqual(allBitsSet, d));

			if(remainder > 0)
			{
				Assert.AreEqual((1 << remainder) - 1, data[fullInts]);
			}
		}

		[Test]
		[SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
		[TestCaseSource(nameof(CopyTo_Hidden_Data))]
		public static void CopyTo_Int_Hidden_Desktop(string label, WireReadyBitArray bits)
		{
			int allBitsSet = unchecked((int)0xffffffff); // 32 bits set to 1 = -1
			int fullInts = bits.Length / BitsPerInt32;
			int remainder = bits.Length % BitsPerInt32;
			int arrayLength = fullInts + (remainder > 0 ? 1 : 0);

			int[] data = new int[arrayLength];
			((ICollection)bits).CopyTo(data, 0);

			Assert.All(data, d => Assert.AreEqual(allBitsSet, d));

		}

		[Test]
		[SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
		[TestCaseSource(nameof(CopyTo_Hidden_Data))]
		public static void CopyTo_Byte_Hidden(string label, WireReadyBitArray bits)
		{
			byte allBitsSet = (1 << BitsPerByte) - 1; // 8 bits set to 1 = 255

			int fullBytes = bits.Length / BitsPerByte;
			int remainder = bits.Length % BitsPerByte;
			int arrayLength = fullBytes + (remainder > 0 ? 1 : 0);

			byte[] data = new byte[arrayLength];
			((ICollection)bits).CopyTo(data, 0);

			Assert.All(data.Take(fullBytes), d => Assert.AreEqual(allBitsSet, d));

			if(remainder > 0)
			{
				Assert.AreEqual((byte)((1 << remainder) - 1), data[fullBytes]);
			}
		}

		[Test]
		[SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
		[TestCaseSource(nameof(CopyTo_Hidden_Data))]
		public static void CopyTo_Byte_Hidden_Desktop(string label, WireReadyBitArray bits)
		{
			byte allBitsSet = (1 << BitsPerByte) - 1; // 8 bits set to 1 = 255

			int fullBytes = bits.Length / BitsPerByte;
			int remainder = bits.Length % BitsPerByte;
			int arrayLength = fullBytes + (remainder > 0 ? 1 : 0);

			byte[] data = new byte[arrayLength];
			((ICollection)bits).CopyTo(data, 0);

			Assert.All(data, d => Assert.AreEqual(allBitsSet, d));

		}*/
	}
}
