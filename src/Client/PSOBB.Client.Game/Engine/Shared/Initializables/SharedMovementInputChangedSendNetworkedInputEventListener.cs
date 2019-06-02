﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class SharedMovementInputChangedSendNetworkedInputEventListener : BaseSingleEventListenerInitializable<IMovementInputChangedEventSubscribable, MovementInputChangedEventArgs>
	{
		private IPeerPayloadSendService<GameClientPacketPayload> SendService { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public SharedMovementInputChangedSendNetworkedInputEventListener(IMovementInputChangedEventSubscribable subscriptionService, [NotNull] IPeerPayloadSendService<GameClientPacketPayload> sendService, [NotNull] ILog logger) 
			: base(subscriptionService)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, MovementInputChangedEventArgs args)
		{
			Logger.Info($"About to send movement change.");

			SendService.SendMessage(new ClientMovementDataUpdateRequest(new Vector2(args.isMoving ? args.NewHorizontalInput : 0.0f, args.isMoving ? args.NewVerticalInput : 0.0f)));
		}
	}
}
