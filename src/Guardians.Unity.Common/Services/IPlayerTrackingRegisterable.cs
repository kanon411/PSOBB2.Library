using System;
using System.Collections.Generic;
using System.Text;
using Dissonance;

namespace Guardians
{
	public interface IPlayerTrackingRegisterable
	{
		void RegisterTracker(IDissonancePlayer player);

		void RemoveTracker(IDissonancePlayer player);
	}
}
