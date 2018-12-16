﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Metadata to indicate a Type is actually an external dependency for
	/// a MonoBehaviour.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ExternalBehaviourAttribute : Attribute
	{
		
	}
}
