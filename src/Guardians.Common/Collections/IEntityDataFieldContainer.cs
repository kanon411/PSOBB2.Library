using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityDataFieldContainer : IReadonlyEntityDataFieldContainer
	{
		void SetFieldValue<TValueType>(int index, TValueType value)
			where TValueType : struct;
	}

	public interface IEntityDataFieldContainer<in TFieldType> : IReadonlyEntityDataFieldContainer<TFieldType>, IEntityDataFieldContainer
	{
		void SetFieldValue<TValueType>(TFieldType index, TValueType value)
			where TValueType : struct;
	}
}
