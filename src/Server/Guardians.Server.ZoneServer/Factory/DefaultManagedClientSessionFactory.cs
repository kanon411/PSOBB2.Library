using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class DefaultManagedClientSessionFactory : IManagedClientSessionFactory
	{
		private ILog Logger { get; }

		private MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>> HandlerService { get; }

		private IRegisterable<int, ZoneClientSession> SessionRegisterable { get; }

		/// <inheritdoc />
		public DefaultManagedClientSessionFactory(
			[NotNull] ILog logger,
			[NotNull] MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>> handlerService,
			[NotNull] IRegisterable<int, ZoneClientSession> sessionRegisterable)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			HandlerService = handlerService ?? throw new ArgumentNullException(nameof(handlerService));
			SessionRegisterable = sessionRegisterable ?? throw new ArgumentNullException(nameof(sessionRegisterable));
		}

		/// <inheritdoc />
		public ManagedClientSession<GameServerPacketPayload, GameClientPacketPayload> Create([NotNull] ManagedClientSessionCreationContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Creating new session. Details: {context.Details}");

			try
			{

				ZoneClientSession clientSession = new ZoneClientSession(context.Client, context.Details, HandlerService, Logger);

				//We should add this to the session collection, and also make sure it is unregistered on disconnection
				SessionRegisterable.Register(context.Details.ConnectionId, clientSession);
				clientSession.OnSessionDisconnection += (source, args) =>
				{
					if(Logger.IsDebugEnabled)
						Logger.Debug($"Session disconnecting. Details: {args.Details} Status: {args.Status}");

					SessionRegisterable.Unregister(source.Details.ConnectionId);
				};

				return clientSession;
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to create Client: {context.Details} Error: {e.Message} \n\n Stack: {e.StackTrace}");

				throw;
			}
		}
	}

	public sealed class ManagedClientSessionCreationContext
	{
		public IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> Client { get; }

		public SessionDetails Details { get; }

		/// <inheritdoc />
		public ManagedClientSessionCreationContext([NotNull] IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> client, [NotNull] SessionDetails details)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			Details = details ?? throw new ArgumentNullException(nameof(details));
		}
	}
}
