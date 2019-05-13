using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	/// <summary>
	/// Contract for types that contain a <see cref="ObjectGuid"/>
	/// </summary>
	public interface IEntityGuidContainer
	{
		/// <summary>
		/// The Network GUID contained inside the container.
		/// </summary>
		ObjectGuid EntityGuid { get; }
	}
}
