using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Dissonance.Audio.Codecs;
using Dissonance.Networking;
using GladNet;
using Guardians;
using Reinterpret.Net;
using UnityEngine;

namespace Dissonance.GladNet
{
	public sealed class GladNetDissonanceClient : GladNetBaseClient<GladNetDissonanceClient, long>, IGameTickable, IVoiceGateway, IVoiceDataProcessor
	{
		private bool isConnected { get; set; }

		private IPeerPayloadSendService<GameClientPacketPayload> SendService { get; }

		/// <inheritdoc />
		public GladNetDissonanceClient(ICommsNetworkState network, [NotNull] IPeerPayloadSendService<GameClientPacketPayload> sendService) 
			: base(network)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
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

					//TODO: We need a way to do this without allocation. And directly write to the stream, like maybe Low Level GladNet3 with (R)UDP.
					byte[] array = new byte[packet.Count];
					Array.Copy(packet.Array, packet.Offset, array, 0, packet.Count);
					NetworkReceivedPacket(new ArraySegment<byte>(array));

					//TODO: Remove unused sequence number.
					SendService.SendMessage(new VoiceDataChangeRaiseRequestPayload(array, 0));
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
			//Nothing, but we may need to do some stuff later.
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

			//TODO: We need to do better senderId rewritting to support more than 255 characters. This won't work for long. Won't even work for alpha.
			ProjectVersionStage.AssertInternalTesting();

			//We need to rewrite the voice packet data
			//It contains the wrong source/sender id. Well, technically it could contain the right ID
			//but we should assume the remote clients are LYING and so we should use the provided entity guid.
			//We don't do this on the server because it's a waste of previous resources
			voiceData.Array[voiceData.Offset + 8] = (byte)entity.EntityId;

			ushort? id = NetworkReceivedPacket(voiceData);

			if(id.HasValue)
				throw new NotSupportedException("We recieved a P2P handshake that we should NEVER recieve. Server should strip these packets.");
		}
	}
}
