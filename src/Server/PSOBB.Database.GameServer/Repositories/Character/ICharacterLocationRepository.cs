using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ICharacterLocationRepository :
		IGenericRepositoryCrudable<int, CharacterLocationModel>
	{
		
	}
}
