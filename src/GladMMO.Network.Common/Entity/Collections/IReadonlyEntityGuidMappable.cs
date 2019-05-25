using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface IReadonlyEntityGuidMappable<TValue> : IReadOnlyDictionary<NetworkEntityGuid, TValue>
	{
		
	}
}
