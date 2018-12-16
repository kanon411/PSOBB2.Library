using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Base-type for <see cref="MonoBehaviour"/>'s
	/// that interact with non-<see cref="MonoBehaviour"/> dependencies.
	/// Mostly the <see cref="MonoBehaviour"/> acts as an object that dispatches engine events
	/// without running any actual logic of its own.
	/// </summary>
	/// <typeparam name="TDependencyType">The dependency type.</typeparam>
	[Injectee]
	public abstract class ExternalizedDependencyMonoBehaviour<TDependencyType> : BaseGuardiansMonoBehaviour
		where TDependencyType : class
	{
		/// <summary>
		/// The external engine dependency that the MonoBehaviour interacts with.
		/// </summary>
		[Inject]
		protected TDependencyType ExternalDependency { get; private set; }
	}
}
