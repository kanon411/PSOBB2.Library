using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public sealed class DefaultThreadUnSafeKnownEntitySet : IKnownEntitySet, IReadonlyKnownEntitySet
	{
		private HashSet<ObjectGuid> InternalKnownSet { get; }

		/// <inheritdoc />
		public DefaultThreadUnSafeKnownEntitySet()
		{
			InternalKnownSet = new HashSet<ObjectGuid>(ObjectGuidEqualityComparer<ObjectGuid>.Instance);
		}

		/// <inheritdoc />
		public void RemoveEntity(ObjectGuid guid)
		{
			if(!isEntityKnown(guid))
				throw new InvalidOperationException($"Cannot removed EntityGuid: {guid} because does not exists in Set: {nameof(DefaultThreadUnSafeKnownEntitySet)}.");

			InternalKnownSet.Remove(guid);
		}

		/// <inheritdoc />
		public void AddEntity(ObjectGuid guid)
		{
			if(isEntityKnown(guid))
				throw new InvalidOperationException($"Cannot add EntityGuid: {guid} because it already exists in Set: {nameof(DefaultThreadUnSafeKnownEntitySet)}.");

			InternalKnownSet.Add(guid);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool isEntityKnown(ObjectGuid guid)
		{
			return InternalKnownSet.Contains(guid);
		}

		/// <inheritdoc />
		public IEnumerator<ObjectGuid> GetEnumerator()
		{
			return InternalKnownSet.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
