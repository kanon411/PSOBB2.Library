using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using UnityEngine;

namespace PSOBB
{
	public sealed class ClientMovementDataUpdateRequestHandler : ControlledEntityRequestHandler<ClientMovementDataUpdateRequest>
	{
		private IEntityGuidMappable<IMovementData> MovementDataMap { get; }

		private IEntityGuidMappable<IMovementGenerator<GameObject>> MovementGenerator { get; }

		/// <inheritdoc />
		public ClientMovementDataUpdateRequestHandler(
			[NotNull] ILog logger, 
			[NotNull] IReadonlyConnectionEntityCollection connectionIdToEntityMap, 
			[NotNull] IEntityGuidMappable<IMovementData> movementDataMap,
			IContextualResourceLockingPolicy<NetworkEntityGuid> lockingPolicy,
			[NotNull] IEntityGuidMappable<IMovementGenerator<GameObject>> movementGenerator) 
			: base(logger, connectionIdToEntityMap, lockingPolicy)
		{
			MovementDataMap = movementDataMap ?? throw new ArgumentNullException(nameof(movementDataMap));
			MovementGenerator = movementGenerator ?? throw new ArgumentNullException(nameof(movementGenerator));
		}

		/// <inheritdoc />
		protected override async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientMovementDataUpdateRequest payload, NetworkEntityGuid guid)
		{
			try
			{
				MovementGenerator[guid] = new ServerPlayerInputChangeMovementGenerator(payload.MovementInput, data => MovementDataMap[guid] = data);
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
