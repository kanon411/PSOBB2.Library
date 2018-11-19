﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class FieldValueUpdateFactory<TDataCollectionType> : IFactoryCreatable<FieldValueUpdate, IChangeTrackableEntityDataCollection>
	{
		/// <inheritdoc />
		public FieldValueUpdate Create(IChangeTrackableEntityDataCollection context)
		{
			if(!context.HasPendingChanges)
				throw new InvalidOperationException($"Cannot create: {nameof(FieldValueUpdate)} from collection with no pending changes.");

			//TODO: This will be a source of contetion. This method will likely be called for N entities meaning lots of allocations and slowdown. We need a better solution
			List<int> changedValues = new List<int>(5); //TODO: What is the best estimate?

			//We just enumerator the changed bits and get the new values in a temporary collection
			var e = context.ChangeTrackingArray.EnumerateSetBitsByIndex();
			while(e.MoveNext())
			{
				int changedValueIndex = e.Current;

				changedValues.Add(context.GetFieldValue<int>(changedValueIndex));
			}

			//TODO: This design is currently going to fail at MASSIVE scale. We need a copyless, allocationless, threadsafe, lockless and serializable solution. A tall order but a MUST in the future.
			return new FieldValueUpdate(new WireReadyBitArray(context.ChangeTrackingArray.InternalIntegerArray.ToArrayTryAvoidCopy()), changedValues.ToArray());
		}
	}
}