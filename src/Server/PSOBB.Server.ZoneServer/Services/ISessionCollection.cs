﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	/// <summary>
	/// Collection of connection/sessions.
	/// </summary>
	public interface ISessionCollection : IReadOnlyCollection<ZoneClientSession>, IRegisterable<int, ZoneClientSession>
	{
		
	}
}
