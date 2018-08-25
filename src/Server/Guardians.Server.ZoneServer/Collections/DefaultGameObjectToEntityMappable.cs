using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	//TODO: Do a better implementation
	public sealed class DefaultGameObjectToEntityMappable : IGameObjectToEntityMappable
	{
		/// <inheritdoc />
		public IReadOnlyDictionary<GameObject, NetworkEntityGuid> ObjectToEntityMap { get; }

		public DefaultGameObjectToEntityMappable()
		{
			ObjectToEntityMap = new ConcurrentDictionary<GameObject, NetworkEntityGuid>();
		}

		/// <inheritdoc />
		public IEnumerator<NetworkEntityGuid> GetEnumerator()
		{
			return ObjectToEntityMap.Values.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
