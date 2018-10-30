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

		private IObjectDestructorable<PlayerSessionDeconstructionContext> SessionDestructor { get; }

		/// <inheritdoc />
		public DefaultManagedClientSessionFactory(
			[NotNull] ILog logger,
			[NotNull] MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>> handlerService,
			[NotNull] IRegisterable<int, ZoneClientSession> sessionRegisterable,
			[NotNull] IObjectDestructorable<PlayerSessionDeconstructionContext> sessionDestructor)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			HandlerService = handlerService ?? throw new ArgumentNullException(nameof(handlerService));
			SessionRegisterable = sessionRegisterable ?? throw new ArgumentNullException(nameof(sessionRegisterable));
			SessionDestructor = sessionDestructor ?? throw new ArgumentNullException(nameof(sessionDestructor));
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

					try
					{
						//TODO: This is kinda a hack, we need this to run on the main thread because it destroys a GameObject
						UnityExtended.UnityMainThreadContext.Post(state =>
						{
							//TODO: Create an async OnSessionDisconnection in GladNet3 so we can handle disconnection logic better.
							SessionDestructor.Destroy(new PlayerSessionDeconstructionContext(context.Details.ConnectionId));
						}, true);
					}
					catch(Exception e)
					{
						if(Logger.IsErrorEnabled)
							Logger.Error($"Error Session Cleanup: {e.Message}\n\nStackTrace: {e.StackTrace}");
						throw;
					}
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
