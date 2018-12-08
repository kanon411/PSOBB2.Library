using System;
using System.Collections.Generic;
using System.Text;
using Dissonance;
using ProtoBuf;

namespace Guardians
{
	//TODO: We actually don't need this, since we want to enforce the same encoding settings. If we allow people to change them they will, and that would be bad for bandwidth
	/// <summary>
	/// Payload sent by the client to initialize the voice chat
	/// session.
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.VoiceInitialization)]
	public sealed class VoiceInitializationRequest : GameClientPacketPayload
	{
		/// <summary>
		/// The settings for the codec.
		/// </summary>
		[ProtoMember(1)]
		public CodecSettings Settings { get; private set; }

		/// <inheritdoc />
		public VoiceInitializationRequest(CodecSettings settings)
		{
			Settings = settings;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private VoiceInitializationRequest()
		{
			
		}
	}
}
