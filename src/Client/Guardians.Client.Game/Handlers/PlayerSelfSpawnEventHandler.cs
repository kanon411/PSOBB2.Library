using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// The handler for the player spawn event
	/// the server sends once it enter's the client into the world.
	/// </summary>
	public sealed class PlayerSelfSpawnEventHandler : BaseZoneClientGameMessageHandler<PlayerSelfSpawnEventPayload>
	{
		private IFactoryCreatable<GameObject, LocalPlayerCreationContext> PlayerFactory { get; }

		/// <inheritdoc />
		public PlayerSelfSpawnEventHandler(ILog logger, IFactoryCreatable<GameObject, LocalPlayerCreationContext> playerFactory)
			: base(logger)
		{

			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
		}

		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, PlayerSelfSpawnEventPayload payload)
		{
			//TODO: Actually handle this. Right now it's just demo code, it actually could fail.
			if(Logger.IsInfoEnabled)
				Logger.Info($"Recieved server commanded PlayerSpawn. Player GUID: {payload.CreationData.EntityGuid} Position: {payload.CreationData.InitialMovementData.CurrentPosition}");

			//Don't do any checks for now, we just spawn
			PlayerFactory.Create(new LocalPlayerCreationContext(payload.CreationData.EntityGuid, payload.CreationData.InitialMovementData));

			return Task.CompletedTask;
		}
	}
}
