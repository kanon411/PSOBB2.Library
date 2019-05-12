using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using GladNet;
using GladMMO;
using SceneJect.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GladMMO
{
	/// <summary>
	/// The component that manages the game network client.
	/// </summary>
	public sealed class GameNetworkClient : BaseUnityNetworkClient<GamePacketPayload, GamePacketPayload>, INetworkClientManager
	{
		/// <inheritdoc />
		public GameNetworkClient(MessageHandlerService<GamePacketPayload, GamePacketPayload> handlers, ILog logger, IPeerMessageContextFactory messageContextFactory)
			: base(handlers, logger, messageContextFactory)
		{

		}
	}
}
