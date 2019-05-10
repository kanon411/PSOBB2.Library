using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	/// <summary>
	/// Metadata to indicate a Type is actually an external dependency for
	/// a MonoBehaviour.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ExternalBehaviourAttribute : Attribute
	{
		
	}
}
