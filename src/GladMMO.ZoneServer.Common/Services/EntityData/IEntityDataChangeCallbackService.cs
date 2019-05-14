using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface IEntityDataChangeCallbackService
	{
		/// <summary>
		/// Invokes any registered change callbacks for the <see cref="entity"/>
		/// if callbacks have been registered for changes involving <see cref="field"/>.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <param name="dataValueAsInt">The new data value as an integer.</param>
		void InvokeChangeEvents(ObjectGuid entity, int field, int dataValueAsInt);
	}
}
