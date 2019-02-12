using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IDirtyableMovementDataCollection : ICollectionMapDirtyable<NetworkEntityGuid>, IReadonlyEntityGuidMappable<IMovementData>
	{
		
	}
}
