using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IDirtyableMovementInformationCollection : ICollectionMapDirtyable<NetworkEntityGuid>, IReadonlyEntityGuidMappable<MovementInformation>
	{
		
	}
}
