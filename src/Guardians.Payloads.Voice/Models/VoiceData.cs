using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Guardians
{
	[ProtoContract]
	public sealed class VoiceData
	{
		[ProtoMember(1)]
		private byte[] _VoiceSegmentData { get; set; }

		//Technically, the longer the session goes on the larger the voice data will get due to
		//protobuf variant encoding.
		/// <summary>
		/// The sequence number for the voice data.
		/// </summary>
		[ProtoMember(2)]
		public uint SequenceNumber { get; private set; }

		[ProtoIgnore]
		public IReadOnlyCollection<byte> VoiceSegmentData => _VoiceSegmentData;

		/// <inheritdoc />
		public VoiceData([NotNull] byte[] voiceSegmentData, uint sequenceNumber)
		{
			_VoiceSegmentData = voiceSegmentData ?? throw new ArgumentNullException(nameof(voiceSegmentData));
			SequenceNumber = sequenceNumber;
		}

		/// <summary>
		/// serializer ctor.
		/// </summary>
		private VoiceData()
		{
			
		}
	}
}
