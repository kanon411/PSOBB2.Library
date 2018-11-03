using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Payload sent by the client to update the server about local client
	/// movement data.
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.MovementDataUpdate)]
	public sealed class ClientMovementDataUpdateRequest : GameClientPacketPayload
	{
		/// <summary>
		/// The movement data to update the server with.
		/// </summary>
		[ProtoMember(1)]
		public IMovementData MovementData { get; }

		/// <inheritdoc />
		public ClientMovementDataUpdateRequest([NotNull] IMovementData movementData)
		{
			MovementData = movementData ?? throw new ArgumentNullException(nameof(movementData));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected ClientMovementDataUpdateRequest()
		{
			
		}
	}
}
