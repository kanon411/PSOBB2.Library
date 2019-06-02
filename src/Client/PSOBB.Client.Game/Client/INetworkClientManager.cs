﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace PSOBB
{
	public interface INetworkClientManager : INetworkClientManager<GameServerPacketPayload, GameClientPacketPayload>
	{

	}

	public interface INetworkClientManager<TIncomingPayloadType, out TOutgoingPayloadType>
		where TOutgoingPayloadType : class
		where TIncomingPayloadType : class
	{
		Task StartHandlingNetworkClient([NotNull] IManagedNetworkClient<TOutgoingPayloadType, TIncomingPayloadType> client);

		Task StopHandlingNetworkClient();
	}
}
