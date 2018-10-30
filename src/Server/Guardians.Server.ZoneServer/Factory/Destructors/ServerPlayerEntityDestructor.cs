using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public sealed class ServerPlayerEntityDestructor : IObjectDestructorable<PlayerEntityDestructionContext>
	{
		private IObjectDestructorable<NetworkEntityGuid> EntityDestructor { get; }

		private IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> GuidToSessionMappable { get; }

		private IEntityGuidMappable<InterestCollection> GuidToInterestCollectionMappable { get; }

		/// <inheritdoc />
		public ServerPlayerEntityDestructor([NotNull] IObjectDestructorable<NetworkEntityGuid> entityDestructor, [NotNull] IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> guidToSessionMappable, [NotNull] IEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable)
		{
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			GuidToSessionMappable = guidToSessionMappable ?? throw new ArgumentNullException(nameof(guidToSessionMappable));
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable ?? throw new ArgumentNullException(nameof(guidToInterestCollectionMappable));
		}

		/// <inheritdoc />
		public bool Destroy(PlayerEntityDestructionContext obj)
		{
			//We have some player specific stuff, especially for the server, that we need to handle
			//removing that regular Entities won't have have the client or even the server.
			GuidToSessionMappable.Remove(obj.EntityGuid);
			GuidToInterestCollectionMappable.Remove(obj.EntityGuid);

			return EntityDestructor.Destroy(obj.EntityGuid);
		}
	}
}
