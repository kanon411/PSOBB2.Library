using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Guardians
{
	/// <summary>
	/// Runs all the crud repo interface default tests against the zone repository.
	/// </summary>
	[TestFixture]
	public class CharacterSessionRepoGuidCrudTests : GenericCrubRepositoryDefaultTests<CharacterDatabaseContext, DatabaseBackedZoneServerRepository, Guid, ZoneInstanceEntryModel>
	{
		public override IEnumerable<Guid> TestCaseKeys { get; } = Enumerable.Range(0, 20).Select(i => Guid.NewGuid()).ToArray();

		/// <inheritdoc />
		public override ZoneInstanceEntryModel BuildRandomModel(bool generateKey)
		{
			return new ZoneInstanceEntryModel(Guid.NewGuid(), GameZoneType.ZoneFirst, "127.0.0.1", 5080);
		}

		/// <inheritdoc />
		public override Guid ProduceKeyFromModel(ZoneInstanceEntryModel model)
		{
			return model.ZoneGuid;
		}
	}
}
