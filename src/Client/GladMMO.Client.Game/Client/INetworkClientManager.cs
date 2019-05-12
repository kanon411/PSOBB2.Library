﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore;
using GladNet;

namespace GladMMO
{
	public interface INetworkClientManager : INetworkClientManager<GamePacketPayload, GamePacketPayload>
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
