using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using PSOBB;
using SceneJect.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace PSOBB
{
	/// <summary>
	/// The component that manages the game network client.
	/// </summary>
	public sealed class GameNetworkClient : BaseUnityNetworkClient<GameServerPacketPayload, GameClientPacketPayload>, INetworkClientManager
	{
		/// <inheritdoc />
		public GameNetworkClient(MessageHandlerService<GameServerPacketPayload, GameClientPacketPayload> handlers, ILog logger, IPeerMessageContextFactory messageContextFactory)
			: base(handlers, logger, messageContextFactory)
		{

		}
	}
}
