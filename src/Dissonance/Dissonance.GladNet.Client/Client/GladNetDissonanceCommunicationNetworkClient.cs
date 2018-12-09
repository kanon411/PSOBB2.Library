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
			}
		}

		/// <inheritdoc />
		protected override void SendUnreliable(ArraySegment<byte> packet)
		{
			byte[] bytes = CreatePacketBufferCopy(packet);

			//Debug parsing to see what the system is sending.
			MessageTypes opCode = (MessageTypes)bytes[2];

			Debug.Log($"Voice Client Attempting to Send: {opCode} UnReliable Magic: {bytes.Reinterpret<short>()}.");

			switch(opCode)
			{
				case MessageTypes.ClientState:
					break;
				case MessageTypes.VoiceData:
					//Spoofing the sent voice data as RECIEVE voice data
					//Just change the client id to 2 to say it's TEST
					packet.Array[packet.Offset + 8] = 2; //sets the LSB of the client id.

					byte[] array = new byte[packet.Count];
					Array.Copy(packet.Array, packet.Offset, array, 0, packet.Count);
					NetworkReceivedPacket(new ArraySegment<byte>(array));
					break;
				case MessageTypes.TextData:
					break;
				case MessageTypes.HandshakeRequest:
					//This is spoofing the handshake request. We're using id 0 so that no other clientid looks like us.
					//remote clients will see our client ID as our player's entity guid.
					//There is actually no reason to write our ID here since the server has to write it in anyway
					//to prevent players from spoofing voice from others.
					PacketWriter packetWriter = new PacketWriter(new byte[DissonanceGladNetConstants.HANDSHAKE_RESPONSE_PACKET_LENGTH]);
					packetWriter.WriteHandshakeResponse(1, 0, new List<ClientInfo<Unit>>(), new Dictionary<string, List<ClientInfo<Unit>>>());
					NetworkReceivedPacket(packetWriter.Written); //this spoofs the handshake response
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
			//TODO: ClientId only supports ushort so eventually, after 65,000 players in the database, we WILL have to make some changes here.
			//TODO: We can make this a lot more efficient.
			ProjectVersionStage.AssertAlpha();
			
			//This spoofs the creation of a state packet that indicates the entity on session 1 joined
			//with named ENITTY_GUID with id EntityGuid using the default Codec
			//in Room Global (only room right now).
			PacketWriter packetWriter2 = new PacketWriter(new byte[100]);
			packetWriter2.WriteClientState(1, entity.RawGuidValue.ToString(), (ushort)entity.EntityId, CodecDefaults.Default, new ReadOnlyCollection<string>(new List<string>() { "Global" }));
			NetworkReceivedPacket(packetWriter2.Written);
		}

		/// <inheritdoc />
		public void LeaveVoiceSession(NetworkEntityGuid entity)
		{
			//TODO: ClientId only supports ushort so eventually, after 65,000 players in the database, we WILL have to make some changes here.
			//TODO: We can make this a lot more efficient.
			ProjectVersionStage.AssertAlpha();

			//TODO: Share buffer
			PacketWriter packetWriter2 = new PacketWriter(new byte[100]);
			packetWriter2.WriteClientState(1, entity.RawGuidValue.ToString(), (ushort)entity.EntityId, CodecDefaults.Default, new ReadOnlyCollection<string>(new List<string>()));
			NetworkReceivedPacket(packetWriter2.Written);
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
