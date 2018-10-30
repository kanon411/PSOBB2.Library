﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class NetworkedEntityDataSaveable : IEntityDataSaveable
	{
		private IReadonlyEntityGuidMappable<MovementInformation> MovementDataMap { get; }

		private IZoneServerToGameServerClient ZoneToSeverClient { get; }

		/// <inheritdoc />
		public NetworkedEntityDataSaveable([NotNull] IReadonlyEntityGuidMappable<MovementInformation> movementDataMap, [NotNull] IZoneServerToGameServerClient zoneToSeverClient)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			ZoneToSeverClient = zoneToSeverClient ?? throw new ArgumentNullException(nameof(zoneToSeverClient));
		}

		/// <inheritdoc />
		public void Save(NetworkEntityGuid guid)
		{
			SaveAsync(guid).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task SaveAsync(NetworkEntityGuid guid)
		{
			//TODO: Check that the entity actually exists
			MovementInformation movementData = MovementDataMap[guid];

			//We can only handle players at the moment, not sure how NPC data would be saved.
			if(guid.EntityType != EntityType.Player)
				return;

			//TODO: Send request to save location/posiion.
		}
	}
}
