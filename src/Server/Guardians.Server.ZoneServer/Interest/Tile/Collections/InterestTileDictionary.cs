using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Guardians
{
	public sealed class InterestTileDictionary : Dictionary<int, InterestTile>, IReadOnlyDictionary<int, IReadonlyInterestCollection>
	{
		public InterestTileDictionary()
		{
		}

		public InterestTileDictionary(IDictionary<int, InterestTile> dictionary, IEqualityComparer<int> comparer) 
			: base(dictionary, comparer)
		{
		}

		public InterestTileDictionary(IDictionary<int, InterestTile> dictionary) 
			: base(dictionary)
		{
		}

		public InterestTileDictionary(IEqualityComparer<int> comparer) 
			: base(comparer)
		{
		}

		public InterestTileDictionary(int capacity) 
			: base(capacity)
		{
		}

		public InterestTileDictionary(int capacity, IEqualityComparer<int> comparer) 
			: base(capacity, comparer)
		{
		}

		/// <inheritdoc />
		IEnumerator<KeyValuePair<int, IReadonlyInterestCollection>> IEnumerable<KeyValuePair<int, IReadonlyInterestCollection>>.GetEnumerator()
		{
			throw new NotSupportedException($"Does not support iterating the readonly dictionary of tiles.");
		}

		/// <inheritdoc />
		public bool TryGetValue(int key, out IReadonlyInterestCollection value)
		{
			bool result = base.TryGetValue(key, out InterestTile t);

			value = t;
			return result;
		}

		/// <inheritdoc />
		IReadonlyInterestCollection IReadOnlyDictionary<int, IReadonlyInterestCollection>.this[int key] => base[key];

		/// <inheritdoc />
		IEnumerable<int> IReadOnlyDictionary<int, IReadonlyInterestCollection>.Keys => base.Keys;

		/// <inheritdoc />
		IEnumerable<IReadonlyInterestCollection> IReadOnlyDictionary<int, IReadonlyInterestCollection>.Values => base.Values;
	}
}
