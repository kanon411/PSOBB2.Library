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
	public class CharacterRepoCrudTests : GenericCrubRepositoryDefaultTests<CharacterDatabaseContext, DatabaseBackedCharacterRepository, int, CharacterEntryModel>
	{
		private static int AccountIdIncrementable = 1;

		/// <inheritdoc />
		public override CharacterEntryModel BuildRandomModel()
		{
			int accountid = Interlocked.Increment(ref AccountIdIncrementable);

			return new CharacterEntryModel(accountid, Guid.NewGuid().ToString());
		}

		/// <inheritdoc />
		public override int ProduceKeyFromModel(CharacterEntryModel model)
		{
			return model.CharacterId;
		}
	}
}
