using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
    public sealed class InMemoryAuthDetailsModelRepository : IAuthDetailsRepository
	{
		/// <summary>
		/// The managed auth details.
		/// </summary>
		private IUserAuthenticationDetailsContainer AuthDetails { get; set; }

		/// <inheritdoc />
		public IUserAuthenticationDetailsContainer Retrieve()
		{
			if(AuthDetails == null)
				return EmptyLoginDetailsModel.Instance;

			return AuthDetails;
		}

		/// <inheritdoc />
		public void SetCurrent(IUserAuthenticationDetailsContainer details)
		{
			AuthDetails = details ?? throw new ArgumentNullException(nameof(details));
		}
	}
}
