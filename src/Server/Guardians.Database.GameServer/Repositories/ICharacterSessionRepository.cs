using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guardians
{
	public interface ICharacterSessionRepository
		: IGenericRepositoryCrudable<int, CharacterSessionModel>
	{
		/// <summary>
		/// Indicates if the account (not a character or characterid)
		/// has an active on-going session.
		/// </summary>
		/// <param name="accountId">The account id to check.</param>
		/// <returns>True if an active session exists for the account id.</returns>
		Task<bool> AccountHasActiveSession(int accountId);

		/// <summary>
		/// Tries to claim an available session with the provided <see cref="characterId"/>
		/// using <see cref="accountId"/> to verify ownership.
		/// </summary>
		/// <param name="accountId"></param>
		/// <param name="characterId"></param>
		/// <returns></returns>
		Task<bool> TryClaimUnclaimedSession(int accountId, int characterId);
	}
}
