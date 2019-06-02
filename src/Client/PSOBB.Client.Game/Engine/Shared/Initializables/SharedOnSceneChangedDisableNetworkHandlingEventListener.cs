﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.PreZoneBurstingScreen)]
	public sealed class SharedOnSceneChangedDisableNetworkHandlingEventListener : BaseSingleEventListenerInitializable<IServerRequestedSceneChangeEventSubscribable, ServerRequestedSceneChangeEventArgs>
	{
		private INetworkClientManager ClientManager { get; }

		/// <inheritdoc />
		public SharedOnSceneChangedDisableNetworkHandlingEventListener(IServerRequestedSceneChangeEventSubscribable subscriptionService, [NotNull] INetworkClientManager clientManager) 
			: base(subscriptionService)
		{
			ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, ServerRequestedSceneChangeEventArgs args)
		{
			//When a scene change is sent by the server, we are DEFINITELY going to change the scene.
			//When we do, we need to export/disconnect the client from it's handling
			//so to do that we just tell the client manager to disconnect.
			ClientManager.StopHandlingNetworkClient().Wait();
		}
	}
}
