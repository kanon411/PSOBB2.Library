using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	[AdditionalRegisterationAs(typeof(IServerRequestedSceneChangeEventSubscribable))]
	[SceneTypeCreate(GameSceneType.PreZoneBurstingScreen)]
	public sealed class LoadNewSceneEventPayloadHandler : BaseZoneClientGameMessageHandler<LoadNewSceneEventPayload>, IServerRequestedSceneChangeEventSubscribable
	{
		/// <inheritdoc />
		public event EventHandler<ServerRequestedSceneChangeEventArgs> OnServerRequestedSceneChange;

		/// <inheritdoc />
		public LoadNewSceneEventPayloadHandler(ILog logger) 
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, LoadNewSceneEventPayload payload)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"InstanceServer requested SceneLoad: {payload.SceneToLoad}");

			//Just dispatch it to any interested parties.
			OnServerRequestedSceneChange?.Invoke(this, new ServerRequestedSceneChangeEventArgs(payload.SceneToLoad));

			return Task.CompletedTask;
		}
	}
}
