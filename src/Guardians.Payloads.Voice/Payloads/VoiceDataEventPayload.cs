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
	public sealed class VoiceDataChangeRaiseEventPayload : GameServerPacketPayload
	{
		/// <summary>
		/// The current voice data for the entity.
		/// (Right now we use GladNet3 TCP so it's ordered, never discarded or stale).
		/// </summary>
		[ProtoMember(1)]
		public EntityAssociatedData<VoiceData> EntityVoiceData { get; private set; }

		/// <inheritdoc />
		public VoiceDataChangeRaiseEventPayload([JetBrains.Annotations.NotNull] EntityAssociatedData<VoiceData> entityVoiceData)
		{
			EntityVoiceData = entityVoiceData ?? throw new ArgumentNullException(nameof(entityVoiceData));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private VoiceDataChangeRaiseEventPayload()
		{
			
		}
	}
}
