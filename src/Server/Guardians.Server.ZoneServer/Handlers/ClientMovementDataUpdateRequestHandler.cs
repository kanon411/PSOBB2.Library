using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class ClientMovementDataUpdateRequestHandler : ControlledEntityRequestHandler<ClientMovementDataUpdateRequest>
	{
		private IEntityGuidMappable<MovementInformation> MovementDataMap { get; }

		/// <inheritdoc />
		public ClientMovementDataUpdateRequestHandler(
			[NotNull] ILog logger, 
			[NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap, 
			[NotNull] IEntityGuidMappable<MovementInformation> movementDataMap) 
			: base(logger, connectionIdToEntityMap)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
		}

		/// <inheritdoc />
		protected override Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientMovementDataUpdateRequest payload, NetworkEntityGuid guid)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Recieved Movement Update for: {guid} with Data: {payload.MovementData.CurrentPosition}");

			//TODO: Handle position data better, we need to actually move the entities.
			MovementDataMap[guid] = payload.MovementData;

			return Task.CompletedTask;
		}
	}
}
