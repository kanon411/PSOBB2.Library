using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IMovementGenerator<in TEntityType> //we make entity type generic so it will be easy to swap between guid/gameobject if needed.
	{
		/// <summary>
		/// Updates the movement for the <see cref="entity"/>
		/// based on the delta from the last update <see cref="deltaTime"/>.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="deltaTime">The deltatime from the last update.</param>
		void Update(TEntityType entity, float deltaTime);
	}
}
