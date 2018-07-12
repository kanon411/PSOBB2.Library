using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityGuidMappable<TValue> : IReadonlyEntityGuidMappable<TValue>, IDictionary<NetworkEntityGuid, TValue>
	{
		
	}
}
