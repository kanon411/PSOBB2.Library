using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
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
			payloadRegister.Register(VoicePayloadMetadataMarker.ClientPayloadTypesByOpcode, VoicePayloadMetadataMarker.ServerPayloadTypesByOpcode);

			//Set the sync context
			UnityExtended.InitializeSyncContext();
		}

		private async Task Start()
		{
			//TODO: Refactor this into a factory.
			long worldId = 0;

			int port = 0;
			try
			{
				//TODO: This is just for testing.
				port = int.Parse(Environment.GetCommandLineArgs().Skip(1).First().Trim('-'));
			}
			catch(Exception e)
			{
				Debug.LogError(e.Message);

				foreach(string line in Environment.GetCommandLineArgs())
					Debug.LogError($"Found Env Line: {line}");
				throw;
			}

			ApplicationBaseContainerPair container = BuildApplicationBase(port);

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
				.OrderBy(tickable =>
				{
					if(tickable.GetType().GetCustomAttribute<GameInitializableOrderingAttribute>() is GameInitializableOrderingAttribute attri)
					{
						return attri.Order;
					}
					else
						return GameInitializableOrderingAttribute.DefaultOrderValue;
				}) //ordering was added because sometimes we want certain tickables to run before others.
				.ToArray();

			//TODO: We need a better way to set all this stuff up
			//We need to start spawning in all the NPCs and stuff.
			IZoneServerToGameServerClient zoneGameServerClient = container.ServiceContainer
				.Resolve<IZoneServerToGameServerClient>();

			try
			{
				//TODO: Fix this mess
				for(ZoneServerRegisterationResponse response = await zoneGameServerClient.RegisterZoneServer(new ZoneServerRegisterationRequest(new ResolvedEndpoint("192.168.0.3", port)))
					.ConfigureAwait(false);; response = await zoneGameServerClient.RegisterZoneServer(new ZoneServerRegisterationRequest(new ResolvedEndpoint("192.168.0.3", port)))
					.ConfigureAwait(false))
				{
					if(response.isSuccessful)
					{
						worldId = response.WorldId;
						break;
					}
					else
						await Task.Delay(1500);
				}

				//This was demo NPC code
				/*var response = await zoneGameServerClient.GetNPCEntriesByMapId(MapId)
					.ConfigureAwait(false);

				//TODO: We're assuming it was successful, which is ok for now but we'll need to handle it failing eventually.
				foreach(var entry in response.Entries)
					Logger.Debug($"NPC Entry Guid: {entry.Guid}");

				//TODO: Eventually we'll need some NPC specific construction, so we won't be able to use the default.
				var entityFactory = container.ServiceContainer
					.Resolve<IFactoryCreatable<GameObject, DefaultEntityCreationContext>>();

				//Join main thread
				await new UnityYieldAwaitable();

				try
				{
					//TODO: Timestamp
					foreach(var entry in response.Entries)
					{
						//TODO: This is test data
						EntityFieldDataCollection<EntityDataFieldType> testData = new EntityFieldDataCollection<EntityDataFieldType>();

						entityFactory.Create(new DefaultEntityCreationContext(entry.Guid, await CreateNpcMovementData(entry, zoneGameServerClient), EntityPrefab.NetworkNpc, testData));
					}
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Exception: {e.Message}\n\nStack: {e.StackTrace}");
					throw;
				}*/
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

		private async Task<IMovementData> CreateNpcMovementData(ZoneServerNpcEntryModel entry, IZoneServerToGameServerClient zoneGameServerClient)
		{
			if(entry.Movement == NpcMovementType.Stationary)
				return new PositionChangeMovementData(DateTime.UtcNow.Ticks, entry.InitialPosition, Vector2.zero);

			if(entry.Movement == NpcMovementType.WapointBased)
			{
				ZoneServerWaypointQueryResponse response = await zoneGameServerClient.GetPathWaypoints(entry.MovementData);

				if(!response.isSuccessful)
					throw new InvalidOperationException($"Failed to create MovementData for Entity: {entry.Guid} Path: {entry.MovementData}");

				return new PathBasedMovementData(response.Waypoints.ToArrayTryAvoidCopy(), DateTime.UtcNow.Ticks);
			}

			throw new InvalidOperationException($"Encountered unhandled MovementType: {entry.Movement}:{(int)entry.Movement}");

			/*return new PathBasedMovementData(new Vector3[]
			{
				new Vector3(2,3,4),
				new Vector3(-5, 1, 5),
				new Vector3(-5, 1, 17),
				new Vector3(10, 1, 17),
				new Vector3(10, 1, 5)
			}, DateTime.UtcNow.Ticks);*/
			//return new PositionChangeMovementData(0, entry.InitialPosition, Vector2.zero);
		}

		//TODO: Create factory/clean this up
		private static ApplicationBaseContainerPair BuildApplicationBase(int port)
		{
			DefaultZoneServerApplicationBaseFactory appBaseFactory = new DefaultZoneServerApplicationBaseFactory();

			return appBaseFactory.Create(new ZoneServerApplicationBaseCreationContext(new UnityLogger(LogLevel.All), new NetworkAddressInfo(IPAddress.Parse("172.31.22.78"), port)));
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
