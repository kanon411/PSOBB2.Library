using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface INetworkTimeService : IReadonlyNetworkTimeService
	{
		/// <summary>
		/// Sets the time syncronization for the network time service.
		/// This will initialize the properties exposed for time usage.
		/// </summary>
		/// <param name="originalLocalTime">The original local time sent in the syncronization packet.</param>
		/// <param name="serverTime">The server time sent back.</param>
		void SetTimeSyncronization(long originalLocalTime, long serverTime);
	}

	public interface IReadonlyNetworkTimeService
	{
		/// <summary>
		/// Caclualtes the RTT (round-trip-time) from a provided initial <see cref="originalLocalTime"/>.
		/// </summary>
		/// <param name="originalLocalTime">The original local time.</param>
		/// <returns>The RTT (2 * latency).</returns>
		long CalculateRoundTripTime(long originalLocalTime);

		/// <summary>
		/// Caclulates the offset between the server and client clocks.
		/// This offset can be used to convert server times to
		/// client times.
		/// </summary>
		/// <param name="originalLocalTime"></param>
		/// <param name="serverTime"></param>
		/// <returns></returns>
		long CalculateTimeOffset(long originalLocalTime, long serverTime);

		/// <summary>
		/// The current computed time offset.
		/// (Does no computation).
		/// </summary>
		long CurrentTimeOffset { get; }

		/// <summary>
		/// The current local time.
		/// (Depending on implementation this could be expensive to call).
		/// </summary>
		long CurrentLocalTime { get; }

		/// <summary>
		/// The current computed latency.
		/// (Does no computation).
		/// </summary>
		long CurrentLatency { get; }
	}
}
