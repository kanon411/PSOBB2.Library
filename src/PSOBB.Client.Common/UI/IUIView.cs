using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IUIView
	{
		/// <summary>
		/// Indicates if the view is enabled.
		/// </summary>
		bool IsEnabled { get; }
	}
}
