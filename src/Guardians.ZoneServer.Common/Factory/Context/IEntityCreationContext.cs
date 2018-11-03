using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityCreationContext : IEntityGuidContainer
	{
		IMovementData MovementData { get; }

		EntityPrefab PrefabType { get; }
	}
}
