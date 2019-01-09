using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Guardians
{
	/// <summary>
	/// Proxy interface for ServerSelection (List) Server RPCs.
	/// </summary>
	[Headers("User-Agent: GuardiansBackend")]
	public interface ISocialServiceToGameServiceClient
	{
		[Headers("Authorization: Bearer")] //see Refits example for tokenless auto-injection to the header.
		[Get("/api/CharacterSession/account/{id}/data")]
		Task<CharacterSessionDataResponse> GetCharacterSessionDataByAccount([AliasAs("id")] int accountId);
	}
}
