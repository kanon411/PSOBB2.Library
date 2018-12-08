using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dissonance.Datastructures;

namespace Dissonance.Networking.Client
{
	internal class GladNetVoiceReceiverManager
	{
		#region fields and properties
		private static readonly Log Log = Logs.Create(LogCategory.Network, typeof(GladNetVoiceReceiverManager).Name);
		private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1.5);

		private readonly EventQueue _events;
		private readonly ConcurrentPool<byte[]> _byteArrayPool;

		private readonly ConcurrentDictionary<string, GladNetRemoteVoiceReceiver> _receivers = new ConcurrentDictionary<string, GladNetRemoteVoiceReceiver>();
		#endregion

		#region constructor
		public GladNetVoiceReceiverManager(EventQueue events, ConcurrentPool<byte[]> byteArrayPool)
		{
			_events = events;
			_byteArrayPool = byteArrayPool;

			_events.OnEnqueuePlayerLeft += OnPlayerLeft;
		}
		#endregion

		private void OnPlayerLeft([NotNull] string name)
		{
			if(_receivers.ContainsKey(name))
			{
				_receivers.TryRemove(name, out var removedReciever);

				var r = removedReciever;
				if(r.Open)
					r.StopSpeaking();
			}
		}

		public void Stop()
		{
			foreach(var reciever in _receivers.Values)
			{
				var r = reciever;
				if(r != null && reciever.Open)
					reciever.StopSpeaking();
			}

			//Discard all receivers
			_receivers.Clear();
		}

		public void Update(DateTime utcNow)
		{
			CheckTimeouts(utcNow);
		}

		/// <summary>
		/// Transition to a non-receiving state for all receivers which have not received any packets within a short window
		/// </summary>
		private void CheckTimeouts(DateTime utcNow)
		{
			foreach(var reciever in _receivers.Values)
				if(reciever != null)
					reciever.CheckTimeout(utcNow, Timeout);
		}

		//TODO: We should not use a string playerid
		public void ReceiveVoiceData(string playerId, ref VoicePacket voicePacket, DateTime? utcNow = null)
		{
			//Create a receiver if there isn't one yet
			if(!_receivers.ContainsKey(playerId))
			{
				//TODO: Redo byte array pooling
				_receivers[playerId] = new GladNetRemoteVoiceReceiver(playerId, _events, _byteArrayPool);
			}

			//Parse the packet with the parser for this remote speaker
			_receivers[playerId].ReceivePacket(ref voicePacket, utcNow ?? DateTime.UtcNow);
		}
	}
}