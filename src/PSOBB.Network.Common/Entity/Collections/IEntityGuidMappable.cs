using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IEntityGuidMappable<TValue> : IDictionary<NetworkEntityGuid, TValue>, IEntityCollectionRemovable
	{
		
	}
}
