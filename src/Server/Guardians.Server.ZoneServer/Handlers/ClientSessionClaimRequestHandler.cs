using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace Guardians
{
	public sealed class ClientSessionClaimRequestHandler : IPeerPayloadSpecificMessageHandler<ClientSessionClaimRequestPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>>
	{
		/// <inheritdoc />
		public async Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, ClientSessionClaimRequestPayload payload)
		{
			//TODO: We should actually send a request to the gameserver to try to validate this. But for now, so we can move on, it remains unimplemented
			await context.PayloadSendService.SendMessage(new ClientSessionClaimResponsePayload(ClientSessionClaimResponseCode.Success))
				.ConfigureAwait(false);
		}
	}
}
