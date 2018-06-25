using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IUIView
	{
		/// <summary>
		/// Indicates if the view is enabled.
		/// </summary>
		bool IsEnabled { get; }
	}
}
