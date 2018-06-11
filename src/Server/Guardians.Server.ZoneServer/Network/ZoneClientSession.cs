using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// The zone client session.
	/// </summary>
	public sealed class ZoneClientSession : ManagedClientSession<GameServerPacketPayload, GameClientPacketPayload>
	{
		/// <summary>
		/// The message handlers service.
		/// </summary>
		private MessageHandlerService<GameClientPacketPayload, GameServerPacketPayload> MessageHandlers { get; }

		/// <inheritdoc />
		public ZoneClientSession(IManagedNetworkServerClient<GameServerPacketPayload, GameClientPacketPayload> internalManagedNetworkClient, 
			SessionDetails details, MessageHandlerService<GameClientPacketPayload, 
			GameServerPacketPayload> messageHandlers) 
			: base(internalManagedNetworkClient, details)
		{
			MessageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
		}

		/// <inheritdoc />
		protected override void OnSessionDisconnected()
		{
			//TODO: Do some cleanup or something someday
		}

		/// <inheritdoc />
		public override Task OnNetworkMessageRecieved(NetworkIncomingMessage<GameClientPacketPayload> message)
		{
			return MessageHandlers.TryHandleMessage(new DefaultPeerMessageContext<GameServerPacketPayload>(this.Connection, this.SendService, null), message);
		}
	}
}
