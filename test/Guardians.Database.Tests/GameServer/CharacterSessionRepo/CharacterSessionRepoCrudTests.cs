using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Guardians
{
	/// <summary>
	/// Runs all the crud repo interface default tests against the character repository.
	/// </summary>
	[TestFixture]
	public class CharacterSessionRepoCrudTests : GenericCrubRepositoryDefaultTests<CharacterDatabaseContext, DatabaseBackedCharacterSessionRepository, int, CharacterSessionModel>
	{
		private static int CharacterIdIncrementable = 1;

		/// <inheritdoc />
		public override CharacterSessionModel BuildRandomModel()
		{
			int id = Interlocked.Increment(ref CharacterIdIncrementable);
			return new CharacterSessionModel(id, 0);
		}

		/// <inheritdoc />
		public override int ProduceKeyFromModel(CharacterSessionModel model)
		{
			return model.CharacterId;
		}
	}
}
