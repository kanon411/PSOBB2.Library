using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Collection of connection/sessions.
	/// </summary>
	public interface ISessionCollection : IReadOnlyCollection<ZoneClientSession>
	{
		
	}
}
