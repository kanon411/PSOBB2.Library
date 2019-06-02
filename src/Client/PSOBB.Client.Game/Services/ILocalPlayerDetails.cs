﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface ILocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; set; }
	}

	public interface IReadonlyLocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; }

		IEntityDataFieldContainer<EntityDataFieldType> EntityData { get; }
	}
}
