using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public interface IAuthenticationClient
	{
		//TODO: Should this be a bool?
		Task<JWTModel> TryAuthenticateAsync(IUserAuthenticationDetailsContainer detailsContainer);
	}
}
