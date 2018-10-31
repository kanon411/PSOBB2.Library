using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Guardians
{
	public sealed class TimeSyncronizationResponseHandler : BaseZoneClientGameMessageHandler<ServerTimeSyncronizationResponsePayload>
	{
		/// <inheritdoc />
		public TimeSyncronizationResponseHandler(ILog logger) 
			: base(logger)
		{

		}

		//TODO: This is a work in progress, we need a time service.
		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, ServerTimeSyncronizationResponsePayload payload)
		{
			long approxLatency = (payload.SentLocalTime - DateTime.UtcNow.Ticks) / 2;

			//time diff is basically the difference between any server timestamps we recieve from the local time.
			//So ServerTime + the timeDiff will be the local time something should be.
			//We remove approxlatency (RTT) from the local client time because we make the assumption that
			//the server actually created the timestamp at AbsoluteServerTime - MessageTravelTime (RTT)
			long timeDiff = (DateTime.UtcNow.Ticks - approxLatency) - payload.ServerTime;

			if(Logger.IsDebugEnabled)
				Logger.Debug($"ApproxLatency: {approxLatency} TimeDiff: {timeDiff}");

			return Task.CompletedTask;
		}
	}
}
