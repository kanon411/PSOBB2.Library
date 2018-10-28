using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	//TODO: Refactor interface implementation
	public sealed class MovementInformationCollection : IDirtyableMovementInformationCollection, IEntityGuidMappable<MovementInformation>
	{
		private EntityGuidDictionary<MovementInformation> InternallyManagedMovementDictionary { get; }

		private Dictionary<NetworkEntityGuid, bool> DirtyChangesTracker { get; }

		public MovementInformationCollection()
		{
			InternallyManagedMovementDictionary = new EntityGuidDictionary<MovementInformation>();
			DirtyChangesTracker = new Dictionary<NetworkEntityGuid, bool>();
		}

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<NetworkEntityGuid, MovementInformation>> GetEnumerator()
		{
			return InternallyManagedMovementDictionary.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)InternallyManagedMovementDictionary).GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(KeyValuePair<NetworkEntityGuid, MovementInformation> item)
		{
			DirtyChangesTracker[item.Key] = true;
			InternallyManagedMovementDictionary.Add(item.Key, item.Value);
		}

		/// <inheritdoc />
		public void Clear()
		{
			DirtyChangesTracker.Clear();
			InternallyManagedMovementDictionary.Clear();
		}

		/// <inheritdoc />
		public bool Contains(KeyValuePair<NetworkEntityGuid, MovementInformation> item)
		{
			return InternallyManagedMovementDictionary.ContainsKey(item.Key) && InternallyManagedMovementDictionary[item.Key] == item.Value;
		}

		/// <inheritdoc />
		public void CopyTo(KeyValuePair<NetworkEntityGuid, MovementInformation>[] array, int arrayIndex)
		{
			throw new NotSupportedException($"TODO: Implement CopyTo.");
		}

		/// <inheritdoc />
		public bool Remove(KeyValuePair<NetworkEntityGuid, MovementInformation> item)
		{
			DirtyChangesTracker.Remove(item.Key);
			//Assume value is right
			return InternallyManagedMovementDictionary.Remove(item.Key);
		}

		/// <inheritdoc />
		int ICollection<KeyValuePair<NetworkEntityGuid, MovementInformation>>.Count => InternallyManagedMovementDictionary.Count;

		/// <inheritdoc />
		bool ICollection<KeyValuePair<NetworkEntityGuid, MovementInformation>>.IsReadOnly => false;

		/// <inheritdoc />
		int IReadOnlyCollection<KeyValuePair<NetworkEntityGuid, MovementInformation>>.Count => InternallyManagedMovementDictionary.Count;

		/// <inheritdoc />
		public void Add(NetworkEntityGuid key, MovementInformation value)
		{
			DirtyChangesTracker[key] = true;
			InternallyManagedMovementDictionary.Add(key, value);
		}

		/// <inheritdoc />
		bool IDictionary<NetworkEntityGuid, MovementInformation>.ContainsKey(NetworkEntityGuid key)
		{
			return InternallyManagedMovementDictionary.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool Remove(NetworkEntityGuid key)
		{
			DirtyChangesTracker.Remove(key);
			return InternallyManagedMovementDictionary.Remove(key);
		}

		/// <inheritdoc />
		bool IDictionary<NetworkEntityGuid, MovementInformation>.TryGetValue(NetworkEntityGuid key, out MovementInformation value)
		{
			return InternallyManagedMovementDictionary.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		bool IReadOnlyDictionary<NetworkEntityGuid, MovementInformation>.ContainsKey(NetworkEntityGuid key)
		{
			return InternallyManagedMovementDictionary.ContainsKey(key);
		}

		/// <inheritdoc />
		bool IReadOnlyDictionary<NetworkEntityGuid, MovementInformation>.TryGetValue(NetworkEntityGuid key, out MovementInformation value)
		{
			return InternallyManagedMovementDictionary.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public MovementInformation this[NetworkEntityGuid key]
		{
			get => InternallyManagedMovementDictionary[key];
			set
			{
				DirtyChangesTracker[key] = true;
				InternallyManagedMovementDictionary[key] = value;
			}
		}

		/// <inheritdoc />
		IEnumerable<NetworkEntityGuid> IReadOnlyDictionary<NetworkEntityGuid, MovementInformation>.Keys => InternallyManagedMovementDictionary.Keys;

		/// <inheritdoc />
		ICollection<MovementInformation> IDictionary<NetworkEntityGuid, MovementInformation>.Values => InternallyManagedMovementDictionary.Values;

		/// <inheritdoc />
		ICollection<NetworkEntityGuid> IDictionary<NetworkEntityGuid, MovementInformation>.Keys => InternallyManagedMovementDictionary.Keys;

		/// <inheritdoc />
		IEnumerable<MovementInformation> IReadOnlyDictionary<NetworkEntityGuid, MovementInformation>.Values => InternallyManagedMovementDictionary.Values;

		/// <inheritdoc />
		public bool isEntryDirty(NetworkEntityGuid key)
		{
			return DirtyChangesTracker.ContainsKey(key) && DirtyChangesTracker[key];
		}

		/// <inheritdoc />
		public void SetDirtyState(NetworkEntityGuid key, bool isDirty)
		{
			DirtyChangesTracker[key] = isDirty;
		}

		/// <inheritdoc />
		public void ClearDirty()
		{
			DirtyChangesTracker.Clear();
		}
	}
}
