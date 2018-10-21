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
		/// <inheritdoc />
		public ClientMovementDataUpdateRequestHandler([NotNull] ILog logger, [NotNull] IReadOnlyDictionary<int, NetworkEntityGuid> connectionIdToEntityMap) 
			: base(logger, connectionIdToEntityMap)
		{
			
		}

		/// <inheritdoc />
		protected override Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientMovementDataUpdateRequest payload, NetworkEntityGuid guid)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Recieved Movement Update for: {guid} with Data: {payload.MovementData.CurrentPosition}");

			return Task.CompletedTask;
		}
	}
}
