using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public sealed class ClientMovementDataUpdateRequestHandler : ControlledEntityRequestHandler<ClientMovementDataUpdateRequest>
	{
		private IEntityGuidMappable<MovementInformation> MovementDataMap { get; }

		private IReadonlyEntityGuidMappable<GameObject> WorldEntities { get; }

		/// <inheritdoc />
		public ClientMovementDataUpdateRequestHandler(
			[NotNull] ILog logger, 
			[NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap, 
			[NotNull] IEntityGuidMappable<MovementInformation> movementDataMap,
			[NotNull] IReadonlyEntityGuidMappable<GameObject> worldEntities) 
			: base(logger, connectionIdToEntityMap)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			WorldEntities = worldEntities ?? throw new ArgumentNullException(nameof(worldEntities));
		}

		/// <inheritdoc />
		protected override async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientMovementDataUpdateRequest payload, NetworkEntityGuid guid)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Recieved Movement Update for: {guid} with Data: {payload.MovementData.CurrentPosition}");

			try
			{
				//TODO: Handle position data better, we need to actually move the entities.
				MovementDataMap[guid] = payload.MovementData;

				//TODO: Make it so that this is simplier to use.
				//We must run this next part on the main thread because it sets a transform.
				await new UnityYieldAwaitable();

				//TODO: This is kinda demo code, directly setting the position of the root object.
				WorldEntities[guid].transform.position = payload.MovementData.CurrentPosition;
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to update MovementData for GUID: {guid}");

				throw;
			}
		}
	}
}
