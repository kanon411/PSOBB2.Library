using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Guardians.Database;
using NUnit.Framework;
using Reinterpret.Net;

namespace Guardians
{
	/// <summary>
	/// Runs all the crud repo interface default tests against the character repository.
	/// </summary>
	[TestFixture]
	public class NpcEntryRepCrudTest : GenericCrubRepositoryDefaultTests<NpcDatabaseContext, DatabaseBackedNpcEntryRepository, int, NPCEntryModel>
	{
		public override IEnumerable<int> TestCaseKeys => new int[] { 1, 2, 3, 5, 6, 7, 8, 9, 22, 6666, short.MaxValue, int.MaxValue };

		/// <inheritdoc />
		public override NPCEntryModel BuildRandomModel()
		{
			Random random = new Random();

			//TODO: IF we ever have a map table we may need to redo the Random map (might fail FK?)
			return new NPCEntryModel(random.Next(), new Vector3<float>(NextRandomFloat(random), NextRandomFloat(random), NextRandomFloat(random)), NextRandomFloat(random), random.Next());
		}

		private static float NextRandomFloat(Random random)
		{
			return random.Next().Reinterpret().Reinterpret<float>();
		}

		/// <inheritdoc />
		public override int ProduceKeyFromModel(NPCEntryModel model)
		{
			return model.EntryId;
		}
	}
}