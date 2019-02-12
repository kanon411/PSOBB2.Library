using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public interface IVoiceDataProcessor
	{
		/// <summary>
		/// Processes the provided voice data.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="voiceData"></param>
		/// <param name="sequenceNumber"></param>
		void ProcessIncomingVoiceData(NetworkEntityGuid entity, ArraySegment<byte> voiceData, uint sequenceNumber);
	}
}
