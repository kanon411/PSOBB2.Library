﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace PSOBB
{
	public sealed class DefaultManagedSessionFactory : IManagedSessionFactory
	{
		private INetworkSerializationService Serializer { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public DefaultManagedSessionFactory([NotNull] INetworkSerializationService serializer, [NotNull] ILog logger)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> Create(ManagedSessionCreationContext context)
		{
			return new DotNetTcpClientNetworkClient(context.Client)
				.AddHeaderlessNetworkMessageReading(Serializer)
				.Build<GameClientPacketPayload, GameServerPacketPayload, IPacketPayload>()
				.AsManagedSession(Logger);
		}
	}
}
