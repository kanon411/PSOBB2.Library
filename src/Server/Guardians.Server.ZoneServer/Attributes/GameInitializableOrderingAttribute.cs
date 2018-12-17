using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GameInitializableOrderingAttribute : Attribute
	{
		/// <summary>
		/// The ordering of the initializable..
		/// </summary>
		public int Order { get; }

		/// <inheritdoc />
		public GameInitializableOrderingAttribute(int order)
		{
			if(order < 0) throw new ArgumentOutOfRangeException(nameof(order));

			Order = order;
		}
	}
}
