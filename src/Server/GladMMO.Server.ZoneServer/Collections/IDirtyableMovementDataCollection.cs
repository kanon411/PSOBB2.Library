using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IDirtyableMovementDataCollection : ICollectionMapDirtyable<NetworkEntityGuid>, IReadonlyEntityGuidMappable<IMovementData>
	{
		
	}
}
