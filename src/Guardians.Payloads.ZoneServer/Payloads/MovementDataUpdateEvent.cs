using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Payload sent by a zone server to update a player about movement data
	/// for entities they may be interested in.
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.MovementDataUpdate)]
	public sealed class MovementDataUpdateEventPayload : GameServerPacketPayload
	{
		/// <summary>
		/// The internally serialized movement data blocks.
		/// </summary>
		[ProtoMember(1)]
		private AssociatedMovementInformation[] _MovementDatas { get; }

		/// <summary>
		/// The movement data sent in the update.
		/// </summary>
		[ProtoIgnore]
		public IReadOnlyCollection<AssociatedMovementInformation> MovementDatas => _MovementDatas;

		/// <summary>
		/// Should always be true. Events should not be sent with no data.
		/// </summary>
		[ProtoIgnore]
		public bool HasMovementData => _MovementDatas != null && _MovementDatas.Length != 0;

		/// <inheritdoc />
		public MovementDataUpdateEventPayload([NotNull] AssociatedMovementInformation[] movementDatas)
		{
			_MovementDatas = movementDatas ?? throw new ArgumentNullException(nameof(movementDatas));
		}

		/// <summary>
		/// Serializer ctor
		/// </summary>
		protected MovementDataUpdateEventPayload()
		{

		}
	}
}
