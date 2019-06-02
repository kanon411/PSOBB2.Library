using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	//Basically, on Start in the lobby scene this initializable will
	//just take the client manager and start the networking pump again
	//from the assumed to be connected and exported managed client.
	[AdditionalRegisterationAs(typeof(INetworkConnectionEstablishedEventSubscribable))]
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class LobbyOnStartRestartNetworkClientHandlingInititablize : IGameInitializable, INetworkConnectionEstablishedEventSubscribable
	{
		/// <summary>
		/// The managed network client that the Unity3D client is implemented on-top of.
		/// </summary>
		private IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> Client { get; }

		private INetworkClientManager ClientManager { get; }

		/// <inheritdoc />
		public event EventHandler OnNetworkConnectionEstablished;

		/// <inheritdoc />
		public LobbyOnStartRestartNetworkClientHandlingInititablize([NotNull] IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> client, [NotNull] INetworkClientManager clientManager)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			//TODO: Should we check if it's still connected?
			await ClientManager.StartHandlingNetworkClient(Client)
				.ConfigureAwait(true); //it's just scene start, it's probably ok to capture the sync context

			OnNetworkConnectionEstablished?.Invoke(this, EventArgs.Empty);
		}
	}
}