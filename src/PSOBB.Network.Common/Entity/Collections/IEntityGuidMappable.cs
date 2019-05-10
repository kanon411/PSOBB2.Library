using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	public interface IEntityGuidMappable<TValue> : IDictionary<NetworkEntityGuid, TValue>, IEntityCollectionRemovable
	{
		
	}
}
