using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;

namespace PSOBB
{
	public interface ILoadedContentResourceHandle
	{
		/// <summary>
		/// Reference-counted in-use count
		/// that representing how many objects are dependent on this
		/// resource being loaded.
		/// </summary>
		[ReadOnly]
		[ShowInInspector]
		int CurrentUseCount { get; }

		/// <summary>
		/// Releases the handle claimed.
		/// </summary>
		void Release();
	}
}
