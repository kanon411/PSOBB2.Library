using System;
using System.Collections.Generic;
using System.Linq;
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

		private IReadonlyEntityGuidMappable<GameObject> GuidToGameObjectMap { get; }

		/// <inheritdoc />
		public ServerPlayerEntityDestructor(
			[NotNull] IObjectDestructorable<NetworkEntityGuid> entityDestructor, 
			[NotNull] IEntityGuidMappable<IPeerPayloadSendService<GameServerPacketPayload>> guidToSessionMappable, 
			[NotNull] IEntityGuidMappable<InterestCollection> guidToInterestCollectionMappable,
			[NotNull] IReadonlyEntityGuidMappable<GameObject> guidToGameObjectMap)
		{
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			GuidToSessionMappable = guidToSessionMappable ?? throw new ArgumentNullException(nameof(guidToSessionMappable));
			GuidToInterestCollectionMappable = guidToInterestCollectionMappable ?? throw new ArgumentNullException(nameof(guidToInterestCollectionMappable));
			GuidToGameObjectMap = guidToGameObjectMap ?? throw new ArgumentNullException(nameof(guidToGameObjectMap));
		}

		/// <inheritdoc />
		public bool Destroy(PlayerEntityDestructionContext obj)
		{
			//TODO: A hack, that removes this entity from other nearby interest lists
			//TODO: This really isn't a good solution, we need something that scales better and that isn't faulty. This could have some odd race conditions to cause ghost entities.
			SphereCollider ourCollider = GuidToGameObjectMap[obj.EntityGuid].GetComponentInChildren<SphereCollider>();
			//This is slow, and hacky. We need a better solution for entities getting deconstructed.
			foreach(var exit in Physics.OverlapSphere(ourCollider.transform.position, ourCollider.radius)
				.Select(c => c.gameObject.transform.root.GetComponentInChildren<InterestRadiusGatewayExit>())
				.Where(e => e != null)
				.Distinct())
			{
				//This simulates us leaving their interest radius.
				exit.OnTriggerExit(ourCollider);
			}

			//We have some player specific stuff, especially for the server, that we need to handle
			//removing that regular Entities won't have have the client or even the server.
			GuidToSessionMappable.Remove(obj.EntityGuid);
			GuidToInterestCollectionMappable.Remove(obj.EntityGuid);

			return EntityDestructor.Destroy(obj.EntityGuid);
		}
	}
}
