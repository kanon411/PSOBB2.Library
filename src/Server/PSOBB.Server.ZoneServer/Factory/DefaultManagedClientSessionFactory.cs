using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace PSOBB
{
	[AdditionalRegisterationAs(typeof(ISessionDisconnectionEventSubscribable))]
	[AdditionalRegisterationAs(typeof(IFactoryCreatable<ManagedClientSession<GameServerPacketPayload, GameClientPacketPayload>, ManagedClientSessionCreationContext>))]
	[AdditionalRegisterationAs(typeof(IManagedClientSessionFactory))]
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class DefaultManagedClientSessionFactory : IManagedClientSessionFactory, ISessionDisconnectionEventSubscribable
	{
		private ILog Logger { get; }

		private MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload, IPeerSessionMessageContext<GameServerPacketPayload>> HandlerService { get; }

		private IRegisterable<int, ZoneClientSession> SessionRegisterable { get; }

		/// <inheritdoc />
		public event EventHandler<SessionStatusChangeEventArgs> OnSessionDisconnection;

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
					OnSessionDisconnection?.Invoke(source, args);
					return Task.CompletedTask;
				};
				/*
				{
					//throw new NotImplementedException("TODO: Redo the whole session disconnection crap.");
					if(Logger.IsDebugEnabled)
						Logger.Debug($"Session disconnecting. Details: {args.Details} Status: {args.Status}");

					try
					{
						//It's possible they disconnected before they even own an entity.
						if(!SessionDestructor.OwnsEntityToDestruct(source.Details.ConnectionId))
						{
							Logger.Debug($"Connection: {source.Details.ConnectionId} disconnecting does not own an entity.");

							//Do NOT return from here, we still want to queue up a removal.
							//The reason being that it's possible they could be disconnecting in the middle of an entity session claim request
							//therefore the main thread cleanup, that runs after spawn, needs to run for this connection id.
						}
						else
						{
							//We know that an entity exists, so we must save it. Before we even queue it up for removal.
							await EntitySaver.SaveAsync(ConnectionCollection[source.Details.ConnectionId])
								.ConfigureAwait(false);
						}

						//All collections and world entity stuff is still intact at this point. We've only taken time to
						//save entity data (maybe on another server/db) and will enqueue actual removal to be handled elsewhere.

						//You may wonder why we're queueing this up for removal. The reason is as follows:
						//It's possible that the player spawn request is enqueued up and to be handled by the EntryManager. We need to handle checking this on the main thread AFTER entry manager has run.
						SessionCleanupQueue.Enqueue(new PlayerSessionDeconstructionContext(source.Details.ConnectionId));
					}
					catch(Exception e)
					{
						if(Logger.IsErrorEnabled)
							Logger.Error($"Error Session Cleanup: {e.Message}\n\nStackTrace: {e.StackTrace}");
						throw;
					}
				};*/

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
