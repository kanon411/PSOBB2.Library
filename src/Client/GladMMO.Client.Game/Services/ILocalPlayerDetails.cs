using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface ILocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; set; }
	}

	public interface IReadonlyLocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; }

		IEntityDataFieldContainer EntityData { get; }
	}
}
