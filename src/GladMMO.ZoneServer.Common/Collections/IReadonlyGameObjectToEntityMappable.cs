using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;
using UnityEngine;

namespace GladMMO
{
	public interface IReadonlyGameObjectToEntityMappable : IEnumerable<ObjectGuid>
	{
		/// <summary>
		/// Dictionary that maps <see cref="GameObject"/> to their owned entit's
		/// <see cref="ObjectGuid"/>
		/// </summary>
		IReadOnlyDictionary<GameObject, ObjectGuid> ObjectToEntityMap { get; }
	}

	public interface IGameObjectToEntityMappable : IEnumerable<ObjectGuid>
	{
		/// <summary>
		/// Dictionary that maps <see cref="GameObject"/> to their owned entit's
		/// <see cref="ObjectGuid"/>
		/// </summary>
		IDictionary<GameObject, ObjectGuid> ObjectToEntityMap { get; }
	}
}
