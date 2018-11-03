using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using GladNet;
using ProtoBuf.Meta;
using UnityEngine;

namespace Guardians
{
	public sealed class ZoneServerBehavior : MonoBehaviour
	{
		private IGameTickable[] GameTickables { get; set; }
		
		private ILog Logger { get; set; }

		//TODO: We need a better way to configure ZoneServer settings.
		[SerializeField]
		public int MapId;

		void Awake()
		{
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);

			//Set the sync context
			UnityExtended.InitializeSyncContext();
		}

		private async Task Start()
		{
			//TODO: Refactor this into a factory.

			ApplicationBaseContainerPair container = BuildApplicationBase();

			Logger = container.ServiceContainer.Resolve<ILog>();

			if(!container.ApplicationBase.StartServer())
			{
				string error = $"Failed to start server on Details: {container.ApplicationBase.ServerAddress}";

				if(container.ApplicationBase.Logger.IsErrorEnabled)
					container.ApplicationBase.Logger.Error(error);

				throw new InvalidOperationException(error);
			}

			GameTickables = container.ServiceContainer
				.Resolve<IEnumerable<IGameTickable>>()
				.ToArray();

			//We need to start spawning in all the NPCs and stuff.
			IZoneServerToGameServerClient zoneGameServerClient = container.ServiceContainer
				.Resolve<IZoneServerToGameServerClient>();

			try
			{
				var response = await zoneGameServerClient.GetNPCEntriesByMapId(MapId)
					.ConfigureAwait(false);

				//TODO: We're assuming it was successful, which is ok for now but we'll need to handle it failing eventually.
				foreach(var entry in response.Entries)
					Logger.Debug($"NPC Entry Guid: {entry.Guid}");

				//TODO: Eventually we'll need some NPC specific construction, so we won't be able to use the default.
				var entityFactory = container.ServiceContainer
					.Resolve<IFactoryCreatable<GameObject, DefaultEntityCreationContext>>();

				//Join main thread
				await new UnityYieldAwaitable();

				//TODO: Timestamp
				foreach(var entry in response.Entries)
					entityFactory.Create(new DefaultEntityCreationContext(entry.Guid, new PositionChangeMovementData(0, entry.InitialPosition, Vector2.zero), EntityPrefab.NetworkNpc));
			}
			catch(Exception e)
			{
				Logger.Error($"Failed: {e.Message}");
				throw;
			}

			await container.ApplicationBase.BeginListening()
				.ConfigureAwait(false);

			if(container.ApplicationBase.Logger.IsWarnEnabled)
				container.ApplicationBase.Logger.Warn($"Server is shutting down.");
		}

		//TODO: Create factory/clean this up
		private static ApplicationBaseContainerPair BuildApplicationBase()
		{
			DefaultZoneServerApplicationBaseFactory appBaseFactory = new DefaultZoneServerApplicationBaseFactory();

			return appBaseFactory.Create(new ZoneServerApplicationBaseCreationContext(new UnityLogger(LogLevel.All), new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 5006)));
		}

		void FixedUpdate()
		{
			if(GameTickables == null || GameTickables.Length == 0)
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"No gametickables; engine skipping tickables.");
				return;
			}

			//We just tick all tickables, they should be order independent
			//This moves the game simulation forward more or less, many things are scheduled to occur
			//via the main game loop on the main game thread
			for(int i = 0; i < GameTickables.Length; i++)
				try
				{
					GameTickables[i].Tick();
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Encountered Exception in Main GameLoop: {e.Message} \n\n Stack: {e.StackTrace}");
				}
		}
	}
}
