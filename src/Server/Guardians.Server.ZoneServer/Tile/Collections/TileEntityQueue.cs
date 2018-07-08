using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class TileEntityQueue : Queue<NetworkEntityGuid>, IDequeable<NetworkEntityGuid>
	{
		/// <inheritdoc />
		public bool isEmpty => Count == 0;

		public TileEntityQueue()
		{

		}

		public TileEntityQueue(IEnumerable<NetworkEntityGuid> collection) 
			: base(collection)
		{

		}

		public TileEntityQueue(int capacity) 
			: base(capacity)
		{

		}
	}
}
