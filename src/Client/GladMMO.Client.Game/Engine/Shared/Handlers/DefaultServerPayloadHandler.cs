using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;
using GladNet;
using UnityEngine;

namespace GladMMO
{
	public sealed class DefaultServerPayloadHandler : IPeerPayloadSpecificMessageHandler<GamePacketPayload, GamePacketPayload>
	{
		/// <inheritdoc />
		public async Task HandleMessage(IPeerMessageContext<GamePacketPayload> context, GamePacketPayload payload)
		{
			Debug.LogWarning($"Recieved unhandled Packet: {payload.GetType().Name}");
		}
	}
}