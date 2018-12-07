using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Payload sent by the client containing voice data it wishes to broadcast.
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.VoiceData)]
	public sealed class VoiceDataChangeRaiseRequest : GameClientPacketPayload
	{
		[ProtoMember(1)]
		private byte[] _VoiceSegmentData { get; set; }

		[ProtoIgnore]
		public IReadOnlyCollection<byte> VoiceSegmentData => _VoiceSegmentData;

		/// <inheritdoc />
		public VoiceDataChangeRaiseRequest(byte[] voiceSegmentData)
		{
			_VoiceSegmentData = voiceSegmentData ?? throw new ArgumentNullException(nameof(voiceSegmentData));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private VoiceDataChangeRaiseRequest()
		{
			
		}
	}
}
