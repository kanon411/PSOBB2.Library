using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dissonance.Datastructures;
using Dissonance.Networking;
using Dissonance.Networking.Client;
using Dissonance.VAD;
using GladNet;
using Guardians;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Dissonance.GladNet
{
	public sealed class GladNetDissonanceCommunicationNetworkClient : ICommsNetwork, IGameTickable, IVoiceGateway, IVoiceDataProcessor
	{
		/// <inheritdoc />
		public ConnectionStatus Status
		{
			get
			{
				//We are NEVER degraded in GladNet. As soon as the connection is open we can send messages
				//This may change those when we implement handshake for header encryption
				if(GladNetConnectionService.isConnected && isInitialized)
					return ConnectionStatus.Connected;
				else
					return ConnectionStatus.Disconnected;
			}
		}

		public bool isInitialized { get; private set; }

		/// <summary>
		/// The GladNet connection service.
		/// </summary>
		private IConnectionService GladNetConnectionService { get; }

		/// <summary>
		/// The network sending service for sending network messages.
		/// </summary>
		private IPeerPayloadSendService<GameClientPacketPayload> NetworkSendService { get; }

		//TODO: We need to let them know we're a client
		/// <inheritdoc />
		public event Action<NetworkMode> ModeChanged;

		/// <inheritdoc />
		public NetworkMode Mode => NetworkMode.Client;

		//This event stuff is from: BaseClient<TServer, TClient, TPeer>
		private readonly EventQueue _events;
		public event Action<string, CodecSettings> PlayerJoined
		{
			add => _events.PlayerJoined += value;
			remove => _events.PlayerJoined -= value;
		}
		public event Action<string> PlayerLeft
		{
			add => _events.PlayerLeft += value;
			remove => _events.PlayerLeft -= value;
		}
		public event Action<RoomEvent> PlayerEnteredRoom
		{
			add => _events.PlayerEnteredRoom += value;
			remove => _events.PlayerEnteredRoom -= value;
		}
		public event Action<RoomEvent> PlayerExitedRoom
		{
			add => _events.PlayerExitedRoom += value;
			remove => _events.PlayerExitedRoom -= value;
		}
		public event Action<VoicePacket> VoicePacketReceived
		{
			add => _events.VoicePacketReceived += value;
			remove => _events.VoicePacketReceived -= value;
		}
		public event Action<string> PlayerStartedSpeaking
		{
			add => _events.PlayerStartedSpeaking += value;
			remove => _events.PlayerStartedSpeaking -= value;
		}
		public event Action<string> PlayerStoppedSpeaking
		{
			add => _events.PlayerStoppedSpeaking += value;
			remove => _events.PlayerStoppedSpeaking -= value;
		}

		private uint SequenceNumber;
		private GladNetVoiceReceiverManager VoiceReciever { get; }

		/// <inheritdoc />
		public GladNetDissonanceCommunicationNetworkClient(
			[NotNull] IConnectionService gladNetConnectionService,
			[NotNull] IPeerPayloadSendService<GameClientPacketPayload> networkSendService)
		{
			GladNetConnectionService = gladNetConnectionService ?? throw new ArgumentNullException(nameof(gladNetConnectionService));
			NetworkSendService = networkSendService ?? throw new ArgumentNullException(nameof(networkSendService));

			ConcurrentPool<byte[]> concurrentPool = new ConcurrentPool<byte[]>(32, () => new byte[1024]);
			_events = new EventQueue(concurrentPool);
			VoiceReciever = new GladNetVoiceReceiverManager(_events, concurrentPool);
		}

		//The BaseCommsNetwork just forwards this request to the BaseClient and the BaseClient
		//just sends this to the VOice
		/// <inheritdoc />
		public void SendVoice(ArraySegment<byte> encodedAudio)
		{
			//This method is called containing an array segement
			//that contains encoded voice data.
			//We should just send this to the server, and the server will deal with relaying the voice data to interested clients.
			if(Status != ConnectionStatus.Connected)
				throw new InvalidOperationException($"Tried to send voice data but not connected.");

			if(encodedAudio.Array == null) throw new ArgumentNullException(nameof(encodedAudio));

			//TODO: We need a way to write this buffer directly on the wire eventually, for performance's sake.
			byte[] array = new byte[encodedAudio.Count];
			Array.Copy(encodedAudio.Array, encodedAudio.Offset, array, 0, encodedAudio.Count);

			
			//We must copy, because the buffer will change.
			NetworkSendService.SendMessage(new VoiceDataChangeRaiseRequestPayload(array, SequenceNumber), DeliveryMethod.UnreliableDiscardStale);
			unchecked { SequenceNumber++; }
		}

		/// <inheritdoc />
		public void Initialize(string playerName,
			Rooms rooms,
			CodecSettings codecSettings)
		{
			//We don't need to send an init request, the server assumes we want voice and the codec settings are authorative and can't be changed.
			//TODO: There is kind of an assumption here that we'll be initializing our voice codec data before we're even in the world
			//We should send the voice init packet here so that the server knows we're ready for voice.
			//NetworkSendService.SendMessage(new VoiceInitializationRequest(codecSettings));
			isInitialized = true;
		}


		/// <inheritdoc />
		public void Tick()
		{
			if(!isInitialized)
				return;

			//TODO: We should migrate this to use the network time.
			VoiceReciever.Update(DateTime.UtcNow);
			_events.DispatchEvents();
		}

		/// <inheritdoc />
		public void JoinVoiceSession(NetworkEntityGuid entity)
		{
			//TODO: Design it so that the voice system uses the entity guid system and not a string.
			_events.EnqueuePlayerJoined(entity.RawGuidValue.ToString(), CodecDefaults.Default); //we don't let others indicate their codec
		}

		/// <inheritdoc />
		public void LeaveVoiceSession(NetworkEntityGuid entity)
		{
			//TODO: Design it so that the voice system uses the entity guid system and not a string.
			_events.EnqueuePlayerLeft(entity.RawGuidValue.ToString());
		}

		/// <inheritdoc />
		public void ProcessIncomingVoiceData(NetworkEntityGuid entity, ArraySegment<byte> voiceData, uint sequenceNumber)
		{
			//TODO: Move away from string based key
			string s = entity.RawGuidValue.ToString();
			VoicePacket packet = new VoicePacket(s, 0.5f, true, voiceData, sequenceNumber);
			VoiceReciever.ReceiveVoiceData(s, ref packet);
		}
	}
}
