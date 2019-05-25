using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public interface INameQueryStorageable
	{
		/// <summary>
		/// Adds an entry for the provided <see cref="entity"/>.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="name"></param>
		void Add(NetworkEntityGuid entity, string name);
	}
}
