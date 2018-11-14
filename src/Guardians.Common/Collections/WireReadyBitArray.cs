
//Orginally based on: https://github.com/dotnet/corefx/blob/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Collections/src/System/Collections/BitArray.cs
//License for the original below, the new version is licensed under the MIT license too.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ProtoBuf;

namespace Guardians
{
	// A vector of bits.  Use this to store bits efficiently, without having to do bit 
	// shifting yourself.
	[ProtoContract]
	[Serializable]
	public sealed class WireReadyBitArray
	{
		// XPerY=n means that n Xs can be stored in 1 Y.
		[ProtoIgnore]
		private const int BitsPerInt32 = 32;

		[ProtoMember(1, IsPacked = true)]
		private int[] InternalArray { get; }

		[ProtoIgnore]
		public IReadOnlyCollection<int> InternalIntegerArray => InternalArray;

		/*=========================================================================
		** Allocates space to hold length bit values. All of the values in the bit
		** array are set to false.
		**
		** Exceptions: ArgumentException if length < 0.
		=========================================================================*/
		public WireReadyBitArray(int length)
			: this(length, false)
		{

		}

		/*=========================================================================
		** Allocates space to hold length bit values. All of the values in the bit
		** array are set to defaultValue.
		**
		** Exceptions: ArgumentOutOfRangeException if length < 0.
		=========================================================================*/
		public WireReadyBitArray(int length, bool defaultValue)
		{
			if(length < 0) throw new ArgumentOutOfRangeException(nameof(length));

			InternalArray = new int[GetArrayLength(length, BitsPerInt32)];

			int fillValue = defaultValue ? unchecked(((int)0xffffffff)) : 0;
			for(int i = 0; i < InternalArray.Length; i++)
			{
				InternalArray[i] = fillValue;
			}
		}

		/*=========================================================================
		** Allocates space to hold the bit values in values. values[0] represents
		** bits 0 - 31, values[1] represents bits 32 - 63, etc. The LSB of each
		** integer represents the lowest index value; values[0] & 1 represents bit
		** 0, values[0] & 2 represents bit 1, values[0] & 4 represents bit 2, etc.
		**
		** Exceptions: ArgumentException if values == null.
		=========================================================================*/
		public WireReadyBitArray(int[] values)
		{
			if(values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			//TODO: This could be slower for some lengths: https://stackoverflow.com/a/33865267
			InternalArray = new int[values.Length];
			Buffer.BlockCopy(values, 0, InternalArray, 0, values.Length * 4);
		}

		/*=========================================================================
		** Allocates a new BitArray with the same length and bit values as bits.
		**
		** Exceptions: ArgumentException if bits == null.
		=========================================================================*/
		public WireReadyBitArray(WireReadyBitArray bits)
		{
			if(bits == null)
			{
				throw new ArgumentNullException(nameof(bits));
			}

			int arrayLength = bits.InternalArray.Length;

			//TODO: This could be slower for some lengths: https://stackoverflow.com/a/33865267
			InternalArray = new int[arrayLength];
			Buffer.BlockCopy(bits.InternalArray, 0, InternalArray, 0, arrayLength * 4);
		}

		public bool this[int index]
		{
			get
			{
				return Get(index);
			}
			set
			{
				Set(index, value);
			}
		}

		/*=========================================================================
		** Returns the bit value at position index.
		**
		** Exceptions: ArgumentOutOfRangeException if index < 0 or
		**             index >= GetLength().
		=========================================================================*/
		public bool Get(int index)
		{
			if(index < 0 || index >= InternalArray.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index), index, $"Provided Index: {index} was not in range. {nameof(InternalArray)} is of Length: {InternalArray.Length}");
			}

			return (InternalArray[index / 32] & (1 << (index % 32))) != 0;
		}

		/*=========================================================================
		** Sets the bit value at position index to value.
		**
		** Exceptions: ArgumentOutOfRangeException if index < 0 or
		**             index >= GetLength().
		=========================================================================*/
		public void Set(int index, bool value)
		{
			if(index < 0 || index >= InternalArray.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index), index, $"Provided Index: {index} was not in range. {nameof(InternalArray)} is of Length: {InternalArray.Length}");
			}

			if(value)
			{
				InternalArray[index / 32] |= (1 << (index % 32));
			}
			else
			{
				InternalArray[index / 32] &= ~(1 << (index % 32));
			}
		}

		/*=========================================================================
		** Sets all the bit values to value.
		=========================================================================*/
		public void SetAll(bool value)
		{
			int fillValue = value ? unchecked(((int)0xffffffff)) : 0;
			for(int i = 0; i < InternalArray.Length; i++)
			{
				InternalArray[i] = fillValue;
			}
		}

		/*=========================================================================
		** Returns a reference to the current instance ANDed with value.
		**
		** Exceptions: ArgumentException if value == null or
		**             value.Length != this.Length.
		=========================================================================*/
		public WireReadyBitArray And(WireReadyBitArray value)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));
			if(InternalArray.Length != value.InternalArray.Length)
				throw new ArgumentException("Lengths are not equivalent.");

			for(int i = 0; i < InternalArray.Length; i++)
			{
				InternalArray[i] &= value.InternalArray[i];
			}

			return this;
		}

		/*=========================================================================
		** Returns a reference to the current instance ORed with value.
		**
		** Exceptions: ArgumentException if value == null or
		**             value.Length != this.Length.
		=========================================================================*/
		public WireReadyBitArray Or(WireReadyBitArray value)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));
			if(InternalArray.Length != value.InternalArray.Length)
				throw new ArgumentException("Lengths are not equivalent.");

			for(int i = 0; i < InternalArray.Length; i++)
			{
				InternalArray[i] |= value.InternalArray[i];
			}

			return this;
		}

		/*=========================================================================
		** Returns a reference to the current instance XORed with value.
		**
		** Exceptions: ArgumentException if value == null or
		**             value.Length != this.Length.
		=========================================================================*/
		public WireReadyBitArray Xor(WireReadyBitArray value)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));
			if(InternalArray.Length != value.InternalArray.Length)
				throw new ArgumentException("Lengths are not equivalent.");

			for(int i = 0; i < InternalArray.Length; i++)
			{
				InternalArray[i] ^= value.InternalArray[i];
			}

			return this;
		}

		/*=========================================================================
		** Inverts all the bit values. On/true bit values are converted to
		** off/false. Off/false bit values are turned on/true. The current instance
		** is updated and returned.
		=========================================================================*/
		public WireReadyBitArray Not()
		{
			int ints = InternalArray.Length;
			for(int i = 0; i < ints; i++)
			{
				InternalArray[i] = ~InternalArray[i];
			}

			return this;
		}

		/// <summary>
		/// Used for conversion between different representations of bit array. 
		/// Returns (n+(div-1))/div, rearranged to avoid arithmetic overflow. 
		/// For example, in the bit to int case, the straightforward calc would 
		/// be (n+31)/32, but that would cause overflow. So instead it's 
		/// rearranged to ((n-1)/32) + 1, with special casing for 0.
		/// 
		/// Usage:
		/// GetArrayLength(77, BitsPerInt32): returns how many ints must be 
		/// allocated to store 77 bits.
		/// </summary>
		/// <param name="n"></param>
		/// <param name="div">use a conversion constant, e.g. BytesPerInt32 to get
		/// how many ints are required to store n bytes</param>
		/// <returns></returns>
		private static int GetArrayLength(int n, int div)
		{
			Debug.Assert(div > 0, "GetArrayLength: div arg must be greater than 0");
			return n > 0 ? (((n - 1) / div) + 1) : 0;
		}
	}
}