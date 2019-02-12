using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class EntityInterestChangeQueue : ConcurrentQueue<EntityInterestChangeContext>, IDequeable<EntityInterestChangeContext>
	{
		/// <inheritdoc />
		public bool isEmpty => base.IsEmpty;

		/// <inheritdoc />
		public EntityInterestChangeContext Dequeue()
		{
			base.TryDequeue(out var temp);
			return temp;
		}
	}
}
