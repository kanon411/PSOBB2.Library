using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for containers that hold
	/// field values.
	/// </summary>
	public interface IEntityDataFieldContainer
	{
		TValueType GetFieldValue<TValueType>(int index)
			where TValueType : struct;
	}
}
