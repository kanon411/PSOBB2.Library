using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;
using GladNet;

namespace GladMMO
{
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionOnStartGameserverAuthenticateInitializable : IGameInitializable
	{
		private ILog Logger { get; }

		private IConnectionService ConnectService { get; }

		/// <summary>
		/// The managed network client that the Unity3D client is implemented on-top of.
		/// </summary>
		private IManagedNetworkClient<GamePacketPayload, GamePacketPayload> Client { get; }

		private INetworkClientManager ClientManager { get; }

		/// <inheritdoc />
		public CharacterSelectionOnStartGameserverAuthenticateInitializable([NotNull] ILog logger, [NotNull] IConnectionService connectService, [NotNull] INetworkClientManager clientManager, [NotNull] IManagedNetworkClient<GamePacketPayload, GamePacketPayload> client)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ConnectService = connectService ?? throw new ArgumentNullException(nameof(connectService));
			ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
			Client = client ?? throw new ArgumentNullException(nameof(client));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			Logger.Info($"Connecting to TrinityCore at: {"127.0.0.1"}:{8085}");

			bool result = await ConnectService.ConnectAsync("127.0.0.1", 8085)
				.ConfigureAwait(false);

			//TODO: Check connection result.
			Logger.Info($"Connection result: {result}");

			Logger.Info($"Starting network message listening.");
			await ClientManager.StartHandlingNetworkClient(Client)
				.ConfigureAwait(false);
		}
	}
}
