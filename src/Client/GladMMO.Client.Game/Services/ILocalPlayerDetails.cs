using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface ILocalPlayerDetails
	{
		ObjectGuid LocalPlayerGuid { get; set; }
	}

	public interface IReadonlyLocalPlayerDetails
	{
		ObjectGuid LocalPlayerGuid { get; }

		IEntityDataFieldContainer<int> EntityData { get; }
	}
}
