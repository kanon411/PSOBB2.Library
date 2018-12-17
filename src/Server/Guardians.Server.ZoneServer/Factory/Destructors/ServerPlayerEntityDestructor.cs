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

		private IReadonlyEntityGuidMappable<GameObject> GuidToGameObjectMap { get; }

		private IReadonlyEntityGuidMappable<InterestCollection> InterestCollections { get; }

		/// <inheritdoc />
		public ServerPlayerEntityDestructor(
			[NotNull] IObjectDestructorable<NetworkEntityGuid> entityDestructor, 
			[NotNull] IReadonlyEntityGuidMappable<GameObject> guidToGameObjectMap,
			[NotNull] IReadonlyEntityGuidMappable<InterestCollection> interestCollections)
		{
			EntityDestructor = entityDestructor ?? throw new ArgumentNullException(nameof(entityDestructor));
			GuidToGameObjectMap = guidToGameObjectMap ?? throw new ArgumentNullException(nameof(guidToGameObjectMap));
			InterestCollections = interestCollections ?? throw new ArgumentNullException(nameof(interestCollections));
		}

		/// <inheritdoc />
		public bool Destroy(PlayerEntityDestructionContext obj)
		{
			//TODO: A hack, that removes this entity from other nearby interest lists
			//TODO: This really isn't a good solution, we need something that scales better and that isn't faulty. This could have some odd race conditions to cause ghost entities.
			ProjectVersionStage.AssertAlpha();

			/*SphereCollider ourCollider = GuidToGameObjectMap[obj.EntityGuid].GetComponentInChildren<SphereCollider>();
			//This is slow, and hacky. We need a better solution for entities getting deconstructed.
			foreach(var exit in Physics.OverlapSphere(ourCollider.transform.position, ourCollider.radius)
				.Select(c => c.gameObject.transform.root.GetComponentInChildren<InterestRadiusGatewayExit>())
				.Where(e => e != null)
				.Distinct())
			{
				//This simulates us leaving their interest radius.
				exit.OnTriggerExit(ourCollider);
			}*/

			bool result = EntityDestructor.Destroy(obj.EntityGuid);

			if(result)
			{
				//To avoid major issues with previous physics based issue we're writing this horriblely slow, hacky solution
				foreach(var ic in InterestCollections.Values)
				{
					if(ic.Contains(obj.EntityGuid))
					{
						//Unregister it, the removal will be queued up and handled in the interest system.
						ic.Unregister(obj.EntityGuid);
					}
				}
			}

			return result;
		}

		/// <inheritdoc />
		public bool OwnsEntityToDestruct(int connectionId)
		{
			throw new NotSupportedException($"TODO: We shouldn't have the destructor actually check this.");
		}
	}
}
