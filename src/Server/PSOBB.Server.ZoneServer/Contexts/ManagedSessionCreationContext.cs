﻿using System;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace PSOBB
{
	public sealed class ManagedSessionCreationContext
	{
		public TcpClient Client { get; }

		/// <inheritdoc />
		public ManagedSessionCreationContext([NotNull] TcpClient client)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
		}
	}
}