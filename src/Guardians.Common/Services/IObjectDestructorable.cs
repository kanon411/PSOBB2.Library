using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for types that provide destruction logic for specific
	/// types of <typeparamref name="TObjectType"/>.
	/// </summary>
	/// <typeparam name="TObjectType">The type that can be destructed.</typeparam>
	public interface IObjectDestructorable<in TObjectType>
	{
		/// <summary>
		/// Attempts to destroy the provided <see cref="obj"/>.
		/// </summary>
		/// <param name="obj">The object to destroy.</param>
		/// <returns>True if the object could be successfully deconstructed.</returns>
		bool Destroy(TObjectType obj);
	}
}
