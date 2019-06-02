﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IEntityCreationContext : IEntityGuidContainer
	{
		IMovementData MovementData { get; }

		EntityPrefab PrefabType { get; }

		/// <summary>
		/// The initial data for the entity.
		/// </summary>
		IEntityDataFieldContainer EntityData { get; }
	}
}
