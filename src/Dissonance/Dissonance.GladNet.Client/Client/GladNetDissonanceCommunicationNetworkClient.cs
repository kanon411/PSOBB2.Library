using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Dissonance.Audio.Codecs;
using Dissonance.Networking;
using Guardians;
using Reinterpret.Net;
using UnityEngine;

namespace Dissonance.GladNet
{
	public sealed class GladNetDissonanceClient : GladNetBaseClient<GladNetDissonanceClient, long>, IGameTickable, IVoiceGateway, IVoiceDataProcessor
	{
		private bool isConnected { get; set; }

		/// <inheritdoc />
		public GladNetDissonanceClient(ICommsNetworkState network) 
			: base(network)
		{
			
		}

		/// <inheritdoc />
		public override void Connect()
		{
			//Just indicates that we're ready for messages.
			isConnected = true;
			Connected();
		}

		/// <inheritdoc />
		protected override void ReadMessages()
		{
			//We recieve messages on a callback so we don't need to do this.
		}

		/// <inheritdoc />
		protected override void SendReliable(ArraySegment<byte> packet)
		{
			byte[] bytes = CreatePacketBufferCopy(packet);

			//Debug parsing to see what the system is sending.
			MessageTypes opCode = (MessageTypes)bytes[2];

			Debug.Log($"Voice Client Attempting to Send: {opCode} Reliable Magic: {bytes.Reinterpret<short>()}.");

			switch(opCode)
			{
				case MessageTypes.ClientState:
					break;
				case MessageTypes.VoiceData:
					break;
				case MessageTypes.TextData:
					break;
				case MessageTypes.HandshakeRequest:
					PacketWriter packetWriter = new PacketWriter(new byte[DissonanceGladNetConstants.HANDSHAKE_RESPONSE_PACKET_LENGTH]);
					packetWriter.WriteHandshakeResponse(1, 1, new List<ClientInfo<Unit>>(), new Dictionary<string, List<ClientInfo<Unit>>>());
					NetworkReceivedPacket(packetWriter.Written); //this spoofs the handshake response

					PacketWriter packetWriter2 = new PacketWriter(new byte[1024]);
					packetWriter2.WriteClientState(1, "Test", 2, CodecDefaults.Default, new ReadOnlyCollection<string>(new List<string>() {"Global"}));

					NetworkReceivedPacket(packetWriter2.Written);

					//TODO: Spoofing a player join.
					//_events.EnqueuePlayerJoined("Test", CodecDefaults.Default);
					//_events.EnqueuePlayerEnteredRoom("Test", "Global", new ReadOnlyCollection<string>(new List<string>()));
					break;
				case MessageTypes.HandshakeResponse:
					break;
				case MessageTypes.ErrorWrongSession:
					break;
				case MessageTypes.ServerRelayReliable:
					break;
				case MessageTypes.ServerRelayUnreliable:
					break;
				case MessageTypes.DeltaChannelState:
					break;
				case MessageTypes.RemoveClient:
					break;
				case MessageTypes.HandshakeP2P:
					break;
			}
		}

		/// <inheritdoc />
		protected override void SendUnreliable(ArraySegment<byte> packet)
		{
			byte[] bytes = CreatePacketBufferCopy(packet);

			//Debug parsing to see what the system is sending.
			MessageTypes opCode = (MessageTypes)bytes[2];

			Debug.Log($"Voice Client Attempting to Send: {opCode} Unreliable Magic: {bytes.Reinterpret<short>()}.");
		}

		private static byte[] CreatePacketBufferCopy(ArraySegment<byte> packet)
		{
			//TODO: We need a way to write this buffer directly on the wire eventually, for performance's sake.
			byte[] array = new byte[packet.Count];
			Array.Copy(packet.Array, packet.Offset, array, 0, packet.Count);

			return array;
		}

		/// <inheritdoc />
		public void Tick()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void JoinVoiceSession(NetworkEntityGuid entity)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void LeaveVoiceSession(NetworkEntityGuid entity)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void ProcessIncomingVoiceData(NetworkEntityGuid entity, ArraySegment<byte> voiceData, uint sequenceNumber)
		{
			//TODO: Is this needed?
			if(!isConnected)
			{
				Debug.LogError($"Recieved messages while not connected to voice.");
				return;
			}

			ushort? id = NetworkReceivedPacket(voiceData);

			if(id.HasValue)
				throw new NotSupportedException("We recieved a P2P handshake that we should NEVER recieve. Server should strip these packets.");
		}
	}
}
