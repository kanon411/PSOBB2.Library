using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class ClientSessionClaimRequestHandler : IPeerPayloadSpecificMessageHandler<ClientSessionClaimRequestPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>
	{
		/// <summary>
		/// The gateway for new player sessions to enter.
		/// </summary>
		private IEntityGateway<PlayerEntitySessionContext> PlayerEntityGatewayEntry { get; }

		/// <inheritdoc />
		public ClientSessionClaimRequestHandler([NotNull] IEntityGateway<PlayerEntitySessionContext> playerEntityGatewayEntry)
		{
			PlayerEntityGatewayEntry = playerEntityGatewayEntry ?? throw new ArgumentNullException(nameof(playerEntityGatewayEntry));
		}

		/// <inheritdoc />
		public async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientSessionClaimRequestPayload payload)
		{
			//TODO: We should actually send a request to the gameserver to try to validate this. But for now, so we can move on, it remains unimplemented
			NetworkEntityGuidBuilder builder = new NetworkEntityGuidBuilder();

			builder
				.WithId(payload.CharacterId)
				.WithType(EntityType.Player);

			//Now that the session has been accepted we should prepare the entry to the world for
			//the session's character
			bool result = PlayerEntityGatewayEntry.TryEntityEnter(new PlayerEntitySessionContext(context.PayloadSendService), builder.Build());

			await context.PayloadSendService.SendMessage(new ClientSessionClaimResponsePayload(ClientSessionClaimResponseCode.Success))
				.ConfigureAwait(false);
		}
	}
}
