using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class DefaultLocalPlayerDetails : ILocalPlayerDetails, IReadonlyLocalPlayerDetails
	{
		/// <inheritdoc />
		public NetworkEntityGuid LocalPlayerGuid { get; set; }

		public DefaultLocalPlayerDetails()
		{
			
		}
	}
}
