using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class UtcNowNetworkTimeService : INetworkTimeService
	{
		/// <inheritdoc />
		public long CurrentTimeOffset { get; private set; }

		/// <inheritdoc />
		public long CurrentLocalTime => DateTime.UtcNow.Ticks;

		/// <inheritdoc />
		public long CurrentLatency { get; private set; }

		/// <inheritdoc />
		public long CurrentRemoteTime => CurrentLocalTime - CurrentLatency - CurrentTimeOffset;

		/// <inheritdoc />
		public long CalculateRoundTripTime(long originalLocalTime)
		{
			return CalculateRoundTripTime(originalLocalTime, CurrentLocalTime);
		}

		private long CalculateRoundTripTime(long originalLocalTime, long currentTicks)
		{
			return (originalLocalTime - currentTicks);
		}

		/// <inheritdoc />
		public long CalculateTimeOffset(long originalLocalTime, long remoteTime)
		{
			long currentTicks = CurrentLocalTime;

			//time diff is basically the difference between any server timestamps we recieve from the local time.
			//So ServerTime + the timeDiff will be the local time something should be.
			//We remove approxlatency (RTT) from the local client time because we make the assumption that
			//the server actually created the timestamp at AbsoluteServerTime - MessageTravelTime (RTT)
			long timeDiff = (currentTicks - CalculateRoundTripTime(originalLocalTime, currentTicks) / 2) - remoteTime;

			return timeDiff;
		}

		/// <inheritdoc />
		public void SetTimeSyncronization(long originalLocalTime, long remoteTime)
		{
			CurrentTimeOffset = CalculateTimeOffset(originalLocalTime, remoteTime);
			CurrentLatency = CalculateRoundTripTime(originalLocalTime) / 2;
		}
	}
}
