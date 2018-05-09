using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ICharacterRepository : 
		IGenericRepositoryCrudable<int, CharacterDatabaseModel>,
		IGenericRepositoryCrudable<string, CharacterDatabaseModel>
	{
		
	}
}
