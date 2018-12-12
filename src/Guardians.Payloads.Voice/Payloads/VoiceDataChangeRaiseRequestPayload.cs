﻿using System;
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
	public sealed class VoiceDataChangeRaiseRequestPayload : GameClientPacketPayload
	{
		/// <summary>
		/// The voice data from the client.
		/// </summary>
		[ProtoMember(1)]
		public VoiceData Data { get; private set; }

		/// <inheritdoc />
		public VoiceDataChangeRaiseRequestPayload([JetBrains.Annotations.NotNull] byte[] voiceSegmentData, uint sequenceNumber)
		{
			if(voiceSegmentData == null) throw new ArgumentNullException(nameof(voiceSegmentData));
			Data = new VoiceData(voiceSegmentData, sequenceNumber);
		}

		/// <inheritdoc />
		public VoiceDataChangeRaiseRequestPayload([JetBrains.Annotations.NotNull] VoiceData data)
		{
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private VoiceDataChangeRaiseRequestPayload()
		{

		}
	}
}