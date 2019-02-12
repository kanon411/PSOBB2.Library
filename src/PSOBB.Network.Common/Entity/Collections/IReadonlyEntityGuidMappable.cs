using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IReadonlyEntityGuidMappable<TValue> : IReadOnlyDictionary<NetworkEntityGuid, TValue>
	{
		
	}
}
