using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Contract for a type of error UI view.
	/// </summary>
    public interface IErrorUIView : IUIView
	{
		/// <summary>
		/// Sets the error message.
		/// </summary>
		/// <param name="errorMessage">The error message to set.</param>
		void SetError(string errorMessage);
	}
}
