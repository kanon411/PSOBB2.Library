using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IRegisterable<in TKey, TValue>
	{
		/// <summary>
		/// Registers the entry with the provided <see cref="key"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The entry.</param>
		void Register(TKey key, TValue value);

		/// <summary>
		/// Checks if the registerable contains an entry
		/// with the key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool Contains(TKey key);

		/// <summary>
		/// Retrieves the value registered.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The value inserted.</returns>
		TValue Retrieve(TKey key);
	}
}
