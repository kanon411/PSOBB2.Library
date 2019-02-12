using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IGuildCharacterMembershipRepository : IGenericRepositoryCrudable<int, CharacterGuildMemberRelationshipModel>
	{
		
	}
}
