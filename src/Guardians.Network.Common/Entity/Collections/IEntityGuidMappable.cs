using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityGuidMappable<TValue> : IDictionary<NetworkEntityGuid, TValue>
	{
		
	}
}
