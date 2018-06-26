using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class EmptyLoginDetailsModel : IUserAuthenticationDetailsContainer
	{
		/// <summary>
		/// Static cached instance.
		/// </summary>
		public static EmptyLoginDetailsModel Instance { get; } = new EmptyLoginDetailsModel();

		/// <inheritdoc />
		public string UserName => String.Empty;

		/// <inheritdoc />
		public string Password => String.Empty;

		public EmptyLoginDetailsModel()
		{
			
		}
	}
}
