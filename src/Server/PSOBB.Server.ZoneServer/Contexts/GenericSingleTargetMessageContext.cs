﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace PSOBB
{
	/// <summary>
	/// Generic message context for sending a <see cref="PayloadToSend"/> to the connection
	/// associated with the Entity <see cref="EntityGuid"/>.
	/// </summary>
	/// <typeparam name="TServerPayloadType"></typeparam>
	public sealed class GenericSingleTargetMessageContext<TServerPayloadType> : IEntityGuidContainer
		where TServerPayloadType : GameServerPacketPayload
	{
		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; }

		public TServerPayloadType PayloadToSend { get; }

		/// <inheritdoc />
		public GenericSingleTargetMessageContext([NotNull] NetworkEntityGuid entityGuid, [NotNull] TServerPayloadType payloadToSend)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			PayloadToSend = payloadToSend ?? throw new ArgumentNullException(nameof(payloadToSend));
		}
	}
}
