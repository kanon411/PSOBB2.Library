using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IEntityCreationContext : IEntityGuidContainer
	{
		MovementInformation MovementData { get; }

		EntityPrefab PrefabType { get; }
	}
}
