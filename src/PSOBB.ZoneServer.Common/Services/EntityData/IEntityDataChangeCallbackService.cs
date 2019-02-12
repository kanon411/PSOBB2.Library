using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityDataChangeCallbackService
	{
		/// <summary>
		/// Invokes any registered change callbacks for the <see cref="entity"/>
		/// if callbacks have been registered for changes involving <see cref="field"/>.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <param name="dataContainer"></param>
		void InvokeChangeEvents(NetworkEntityGuid entity, EntityDataFieldType field, IEntityDataFieldContainer dataContainer);
	}
}
