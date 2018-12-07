using System;
using ProtoBuf;

namespace Guardians.Payloads.Voice
{
	/// <summary>
	/// Payload sent by the client to update the server about local client
	/// movement data.
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.VoiceInitialization)]
	public sealed class VoiceInitializationReadyEvent : GameServerPacketPayload
	{
		//We don't need to send entity guid here
		//the client should know itself by now.
		//We just need to load the client know that the server is ready for initialization.
		public VoiceInitializationReadyEvent()
		{
			
		}
	}
}
