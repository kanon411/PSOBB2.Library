using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class PlayerEntitySessionContext
	{
		public IPeerPayloadSendService<GameServerPacketPayload> ZoneSession { get; }

		/// <inheritdoc />
		public PlayerEntitySessionContext([NotNull] IPeerPayloadSendService<GameServerPacketPayload> zoneSession)
		{
			ZoneSession = zoneSession ?? throw new ArgumentNullException(nameof(zoneSession));
		}
	}
}
