using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
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

		/// <summary>
		/// Checks if the provided <see cref="connectionId"/>
		/// owns an entity that needs to be destructed.
		/// </summary>
		/// <param name="connectionId">The connection id to check.</param>
		/// <returns>True if an entity is owned that needs to be destroyed.</returns>
		bool OwnsEntityToDestruct(int connectionId);
	}
}
