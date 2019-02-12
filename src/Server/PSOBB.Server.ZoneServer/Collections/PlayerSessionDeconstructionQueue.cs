using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class PlayerSessionDeconstructionQueue : ConcurrentQueue<PlayerSessionDeconstructionContext>, IDequeable<PlayerSessionDeconstructionContext>
	{
		/// <inheritdoc />
		public bool isEmpty => ((ConcurrentQueue<PlayerSessionDeconstructionContext>)this).IsEmpty;

		/// <inheritdoc />
		public PlayerSessionDeconstructionContext Dequeue()
		{
			this.TryDequeue(out var temp);
			return temp;
		}
	}
}
