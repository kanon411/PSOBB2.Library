﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace PSOBB
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class EntityDataCollectionTestRangeAttribute : RangeAttribute
	{
		/// <inheritdoc />
		public EntityDataCollectionTestRangeAttribute() 
			: base(0, Enum.GetValues(typeof(TestFieldType)).Length - 1)
		{

		}
	}
}
