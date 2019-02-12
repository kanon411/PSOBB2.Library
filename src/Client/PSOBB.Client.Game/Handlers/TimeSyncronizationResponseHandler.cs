using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	public sealed class TimeSyncronizationResponseHandler : BaseZoneClientGameMessageHandler<ServerTimeSyncronizationResponsePayload>
	{
		private INetworkTimeService TimeService { get; }

		/// <inheritdoc />
		public TimeSyncronizationResponseHandler(ILog logger, INetworkTimeService timeService) 
			: base(logger)
		{
			TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
		}

		//TODO: This is a work in progress, we need a time service.
		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, ServerTimeSyncronizationResponsePayload payload)
		{
			TimeService.SetTimeSyncronization(payload.SentLocalTime, payload.ServerTime);

			if(Logger.IsDebugEnabled)
				Logger.Debug($"ApproxLatency: {TimeService.CurrentLatency / TimeSpan.TicksPerMillisecond} ms TimeDiff: {TimeService.CurrentTimeOffset / TimeSpan.TicksPerMillisecond} ms");

			return Task.CompletedTask;
		}
	}
}
