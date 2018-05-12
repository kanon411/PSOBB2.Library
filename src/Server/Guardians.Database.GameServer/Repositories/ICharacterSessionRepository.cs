using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface ICharacterSessionRepository
		: IGenericRepositoryCrudable<int, CharacterSessionModel>
	{

	}
}
