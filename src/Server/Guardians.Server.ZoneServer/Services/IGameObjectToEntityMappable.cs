using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public interface IGameObjectToEntityMappable : IEnumerable<NetworkEntityGuid>
	{
		/// <summary>
		/// Dictionary that maps <see cref="GameObject"/> to their owned entit's
		/// <see cref="NetworkEntityGuid"/>
		/// </summary>
		IReadOnlyDictionary<GameObject, NetworkEntityGuid> ObjectToEntityMap { get; }
	}
}
