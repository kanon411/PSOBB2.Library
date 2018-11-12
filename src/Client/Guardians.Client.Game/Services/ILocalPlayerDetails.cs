using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ILocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; set; }
	}

	public interface IReadonlyLocalPlayerDetails
	{
		NetworkEntityGuid LocalPlayerGuid { get; }
	}
}
