using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class InterestChangedPacketBuilder
	{
		/// <summary>
		/// The movement data mappabled from an entity guid.
		/// </summary>
		private IReadonlyEntityGuidMappable<MovementInformation> MovementMapper { get; }

		/// <inheritdoc />
		public InterestChangedPacketBuilder([NotNull] IReadonlyEntityGuidMappable<MovementInformation> movementMapper)
		{
			MovementMapper = movementMapper ?? throw new ArgumentNullException(nameof(movementMapper));
		}

		public NetworkObjectVisibilityChangeEventPayload Build([NotNull] IReadOnlyCollection<NetworkEntityGuid> joining, [NotNull] IReadOnlyCollection<NetworkEntityGuid> leaving)
		{
			if(joining == null) throw new ArgumentNullException(nameof(joining));
			if(leaving == null) throw new ArgumentNullException(nameof(leaving));

			if(joining.Count == 0 && leaving.Count == 0)
				throw new InvalidOperationException($"Cannot build interest change packet when changes are empty.");

			//Arrange packet data
			EntityCreationData[] creationDatas = joining
				.Select(j => new EntityCreationData(j, MovementMapper[j]))
				.ToArray();

			NetworkObjectVisibilityChangeEventPayload changePayload = new NetworkObjectVisibilityChangeEventPayload(creationDatas, leaving.ToArray());

			return changePayload;
		}
	}
}
