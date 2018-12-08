using System;
using System.Collections.Generic;
using Dissonance.Audio.Playback;
using Dissonance.Datastructures;
using Dissonance.Extensions;

namespace Dissonance.Networking.Client
{
	/// <summary>
	/// All the state to do with a remote player we are receiving audio from
	/// </summary>
	internal class GladNetRemoteVoiceReceiver
	{
		#region fields and properties
		private static readonly Log Log = Logs.Create(LogCategory.Network, typeof(GladNetRemoteVoiceReceiver).Name);

		private readonly string _name;
		public string Name { get { return _name; } }

		private readonly EventQueue _events;
		private readonly ConcurrentPool<byte[]> _byteArrPool;

		private DateTime _lastReceiptTime;
		private ushort _remoteSequenceNumber;
		private uint _localSequenceNumber;

		public bool Open { get; private set; }

		private int _currentChannelSession;
		#endregion

		#region constructor
		public GladNetRemoteVoiceReceiver(string remoteName, EventQueue events, ConcurrentPool<byte[]> byteArrPool)
		{
			_name = remoteName;
			_events = events;
			_byteArrPool = byteArrPool;
		}
		#endregion

		/// <summary>
		/// Stop speaking if we've gone for too long without any packets from this peer
		/// </summary>
		/// <param name="utcNow"></param>
		/// <param name="timeout"></param>
		public void CheckTimeout(DateTime utcNow, TimeSpan timeout)
		{
			//Early exit if we're not receiving (in which case we can't possibly timeout)
			if(!Open)
				return;

			if((utcNow - _lastReceiptTime) > timeout)
			{
				Log.Debug("Client '{0}' timed out active speech session", Name);
				StopSpeaking();
			}
		}

		#region start/stop
		public void StopSpeaking()
		{
			Log.AssertAndThrowPossibleBug(Open, "E8A0D33E-8C74-45F9-AA8C-3889012498D7", "Attempted to stop speaking when not speaking");

			Open = false;
			_events.EnqueueStoppedSpeaking(Name);
		}

		private void StartSpeaking(ushort startSequenceNumber, DateTime utcNow)
		{
			Log.AssertAndThrowPossibleBug(!Open, "E8A0D33E-8C74-45F9-AA8C-3889012498D7", "Attempted to start speaking when already speaking");

			// Start speaking, setup up all the speech stream data
			_remoteSequenceNumber = startSequenceNumber;
			_localSequenceNumber = 0;
			_lastReceiptTime = utcNow;
			Open = true;

			_events.EnqueueStartedSpeaking(Name);
		}
		#endregion

		public void ReceivePacket(ref VoicePacket voicePacket, DateTime utcNow)
		{
			ushort sequenceNumber = (ushort)voicePacket.SequenceNumber;

			//Read the list of channels this voice data is being broadcast on and accumulate info about the channels we're listening on

			//Update the statistics for the channel this data is coming in over and then if it's successful Send voice data onwards
			if(UpdateSpeakerState(sequenceNumber, utcNow))
			{
				//Copy voice data into another buffer (we can't keep hold of this one, it will be recycled after we finish processing this packet)
				var frame = voicePacket.EncodedAudioFrame.CopyTo(_byteArrPool.Get());

				//Send the event (buffer will be recycled after the event has been dispatched)
				_events.EnqueueVoiceData(new VoicePacket(
					Name, 0.5f, false,
					frame, _localSequenceNumber));
			}
		}

		private bool UpdateSpeakerState(ushort sequenceNumber, DateTime utcNow)
		{
			//If necessary start speaking (don't bother if allClosing, because that would create a 1 packet session, not much point to that)
			if(!Open)
				StartSpeaking(sequenceNumber, utcNow);

			//If we're now in a speech session update it
			if(Open)
			{
				//Update the sequence number (discard packet if we fail for some reason)
				if(!UpdateSequenceNumber(sequenceNumber, utcNow))
					return false;
			}

			return Open;
		}

		private bool UpdateSequenceNumber(ushort sequenceNumber, DateTime utcNow)
		{
			// We assume that the first packet we receive for a session is the first packet of that session, and it accordingly gets assigned a sequence number...
			// ...of zero. Of course it's possible that the true first packet arrives late, however this would cause a negative sequence number and generally...
			// ...cause chaos. We discard the packet (losing the first 10-40ms of speech in this circumstance).
			var sequenceDelta = _remoteSequenceNumber.WrappedDelta(sequenceNumber);
			if(_localSequenceNumber + sequenceDelta < 0)
			{
				Log.Trace("Discarding old packet which would cause negative sequence number");
				return false;
			}

			//Push forward the sequence number
			_localSequenceNumber = (uint)(_localSequenceNumber + sequenceDelta);
			_remoteSequenceNumber = sequenceNumber;
			_lastReceiptTime = utcNow;

			return true;
		}
	}
}