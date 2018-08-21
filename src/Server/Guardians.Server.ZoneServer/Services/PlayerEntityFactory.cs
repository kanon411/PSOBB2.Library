using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public sealed class PlayerEntityFactory : IFactoryCreatable<GameObject, PlayerEntityCreationContext>
	{
		private IEntityGuidMappable<GameObject> GuidToGameObjectMappable { get; }

		private IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> GuidToSessionMappable { get; }

		private IEntityGuidMappable<InterestCollection> GuidToInterestCollectionMappable { get; }

		private IEntityGuidMappable<MovementInformation> GuidToMovementInfoMappable { get; }

		/// <inheritdoc />
		public PlayerEntityFactory([NotNull] IEntityGuidMappable<GameObject> guidToGameObjectMappable, [NotNull] IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> guidToSessionMappable, [NotNull] IEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable, [NotNull] IEntityGuidMappable<MovementInformation> guidToMovementInfoMappable)
		{
			GuidToGameObjectMappable = guidToGameObjectMappable ?? throw new ArgumentNullException(nameof(guidToGameObjectMappable));
			GuidToSessionMappable = guidToSessionMappable ?? throw new ArgumentNullException(nameof(guidToSessionMappable));
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable ?? throw new ArgumentNullException(nameof(guidToInterestCollectionMappable));
			GuidToMovementInfoMappable = guidToMovementInfoMappable ?? throw new ArgumentNullException(nameof(guidToMovementInfoMappable));
		}

		/// <inheritdoc />
		public GameObject Create(PlayerEntityCreationContext context)
		{
			//TODO: Implement this
			GuidToSessionMappable.Add(context.EntityGuid, context.SessionContext.ZoneSession);

			//When we create the actual world represenation of the
			//player we need to add all the entity guid mappable references
			//to it as well.
			return null;
		}
	}
}
