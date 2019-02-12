using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IReadonlyEntityGuidMappable<TValue> : IReadOnlyDictionary<NetworkEntityGuid, TValue>
	{
		
	}
}
