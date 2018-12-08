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
	public sealed class TestGladNetDissonanceCommunicationNetworkClient : MonoBehaviour, ICommsNetwork, ISession
	{
		/// <inheritdoc />
		public ConnectionStatus Status
		{
			get
			{
				//We are NEVER degraded in GladNet. As soon as the connection is open we can send messages
				//This may change those when we implement handshake for header encryption
				if(true)
					return ConnectionStatus.Connected;
				else
					return ConnectionStatus.Disconnected;
			}
		}

		/// <summary>
		/// The GladNet connection service.
		/// </summary>
		private IConnectionService GladNetConnectionService { get; }

		/// <summary>
		/// The network sending service for sending network messages.
		/// </summary>
		private IPeerPayloadSendService<GameClientPacketPayload> NetworkSendService { get; }

		/// <summary>
		/// The voice subscription service.
		/// Subscribe to this get to voice activision change events.
		/// (Not called on the main thread)
		/// </summary>
		public IVoiceActivisionSubscribable VoiceChangeSubscribable { get; }

		//TODO: We need to let them know we're a client
		/// <inheritdoc />
		public event Action<NetworkMode> ModeChanged;

		/// <inheritdoc />
		public NetworkMode Mode => NetworkMode.Client;

		/// <inheritdoc />
		public event Action<string, CodecSettings> PlayerJoined;

		/// <inheritdoc />
		public event Action<string> PlayerLeft;

		/// <inheritdoc />
		public event Action<VoicePacket> VoicePacketReceived;

		/// <inheritdoc />
		public event Action<string> PlayerStartedSpeaking;

		/// <inheritdoc />
		public event Action<string> PlayerStoppedSpeaking;

		/// <inheritdoc />
		public event Action<RoomEvent> PlayerEnteredRoom;

		/// <inheritdoc />
		public event Action<RoomEvent> PlayerExitedRoom;

		[SerializeField]
		private DissonanceComms Comms;

		[SerializeField]
		private GladNetTestPlayer Player;

		private uint SequenceNumber;
		private GladNetVoiceReceiverManager VoiceReciever { get; set; }

		private EventQueue _events;

		/// <inheritdoc />
		public TestGladNetDissonanceCommunicationNetworkClient(
			[NotNull] IConnectionService gladNetConnectionService,
			[NotNull] IPeerPayloadSendService<GameClientPacketPayload> networkSendService, 
			[NotNull] IVoiceActivisionSubscribable voiceChangeSubscribable)
		{
			GladNetConnectionService = gladNetConnectionService ?? throw new ArgumentNullException(nameof(gladNetConnectionService));
			NetworkSendService = networkSendService ?? throw new ArgumentNullException(nameof(networkSendService));
			VoiceChangeSubscribable = voiceChangeSubscribable ?? throw new ArgumentNullException(nameof(voiceChangeSubscribable));

			ConcurrentPool<byte[]> concurrentPool = new ConcurrentPool<byte[]>(200, () => new byte[1024]);
			_events = new EventQueue(concurrentPool);

			VoiceReciever = new GladNetVoiceReceiverManager(_events, concurrentPool);
		}

		//The BaseCommsNetwork just forwards this request to the BaseClient and the BaseClient
		//just sends this to the VOice
		/// <inheritdoc />
		public void SendVoice(ArraySegment<byte> encodedAudio)
		{
			Debug.Log("Calling");

			//This method is called containing an array segement
			//that contains encoded voice data.
			//We should just send this to the server, and the server will deal with relaying the voice data to interested clients.
			if(Status != ConnectionStatus.Connected)
				throw new InvalidOperationException($"Tried to send voice data but not connected.");

			if(encodedAudio.Array == null) throw new ArgumentNullException(nameof(encodedAudio));

			//Write the packet
			/*var packet = new PacketWriter(_sender.SendBufferPool.Get())
				.WriteVoiceData(_session.SessionId, _session.LocalId.Value, _sequenceNumber, _channelSessionId, openChannels, encodedAudio)
				.Written;*/
			//TODO: Create and queue audio data packet
			//TODO: Deal with sequenceing
			//unchecked { _sequenceNumber++; }

			//TODO: Is it better to copy? Or to send the data directly on the main thread? Allocation vs MainThread time. Copying also takes time too.
			//We must copy, because the buffer will change.
			//NetworkSendService.SendMessage(new VoiceDataChangeRaiseRequest(encodedAudio.ToArray(), SequenceNumber), DeliveryMethod.UnreliableDiscardStale);

			byte[] array = new byte[encodedAudio.Count];
			Array.Copy(encodedAudio.Array, encodedAudio.Offset, array, 0, encodedAudio.Count);

			var voicePacket = new VoicePacket("Test", 0.5f, true, new ArraySegment<byte>(array), SequenceNumber);
			VoiceReciever.ReceiveVoiceData("Test", ref voicePacket);
			unchecked { SequenceNumber++; }
		}

		private string tempPlayerName;

		private CodecSettings tempCodecSettings;

		private object syncObj;
		private Queue<byte[]> AudioQueue;

		private Queue<Action> ActionQueue;

		/// <inheritdoc />
		public void Initialize(string playerName, 
			Rooms rooms,
			CodecSettings codecSettings)
		{
			tempPlayerName = playerName;
			tempCodecSettings = codecSettings;
			AudioQueue = new Queue<byte[]>();
			syncObj = new object();
			ActionQueue = new Queue<Action>();

			ConcurrentPool<byte[]> concurrentPool = new ConcurrentPool<byte[]>(200, () => new byte[1024]);
			_events = new EventQueue(concurrentPool);

			VoiceReciever = new GladNetVoiceReceiverManager(_events, concurrentPool);

			StartCoroutine(StartEnumerator());

			Debug.Log($"Code: {codecSettings.Codec} FrameSize: {codecSettings.FrameSize} SampleRate: {codecSettings.SampleRate}");
		}

		private IEnumerator StartEnumerator()
		{
			while(true)
			{
				yield return null;
				//This does not process the packets, it only does bookkeeping per the internal doc/comments about this
				VoiceReciever.Update(DateTime.UtcNow);
				_events.DispatchEvents();
			}
		}

		private async Task Start()
		{
			await Task.Delay(2000)
				.ConfigureAwait(true);

			lock(syncObj)
			{
				_events.EnqueuePlayerJoined("Test", tempCodecSettings);
				_events.EnqueuePlayerEnteredRoom("Test", "Global", null);
				_events.VoicePacketReceived += this.VoicePacketReceived;
				_events.PlayerEnteredRoom += this.PlayerEnteredRoom;
				_events.PlayerEnteredRoom += e => Comms.TrackPlayerPosition(Player);
				_events.PlayerJoined += this.PlayerJoined;
				_events.PlayerStoppedSpeaking += this.PlayerStoppedSpeaking;
				_events.PlayerStartedSpeaking += this.PlayerStartedSpeaking;
			}
		}

		/// <inheritdoc />
		public uint SessionId => throw new NotSupportedException("TODO: What is this?");

		/// <inheritdoc />
		public ushort? LocalId => throw new NotSupportedException("TODO: What is this?");

		/// <inheritdoc />
		public string LocalName => tempPlayerName;
	}
}
