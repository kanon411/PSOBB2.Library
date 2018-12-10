using System;
using System.Collections.Generic;
using System.Text;
using Guardians;

namespace Dissonance.GladNet
{
	public sealed class GladNetDissonanceClientCurrentInstanceAdapter : IVoiceGateway, IVoiceDataProcessor
	{
		private object syncObj = new object();
		private GladNetDissonanceClient _clientInstance;

		/// <summary>
		/// Mutable current client instance.
		/// </summary>
		public GladNetDissonanceClient ClientInstance
		{
			get
			{
				lock(syncObj)
					return _clientInstance;
			}
			set
			{
				lock(syncObj)
					_clientInstance = value;
			}
		}


		/// <inheritdoc />
		public void JoinVoiceSession(NetworkEntityGuid entity)
		{
			ClientInstance.JoinVoiceSession(entity);
		}

		/// <inheritdoc />
		public void LeaveVoiceSession(NetworkEntityGuid entity)
		{
			ClientInstance.LeaveVoiceSession(entity);
		}

		/// <inheritdoc />
		public void ProcessIncomingVoiceData(NetworkEntityGuid entity, ArraySegment<byte> voiceData, uint sequenceNumber)
		{
			ClientInstance.ProcessIncomingVoiceData(entity, voiceData, sequenceNumber);
		}
	}
}
