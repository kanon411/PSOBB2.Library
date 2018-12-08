using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using Common.Logging.Simple;
using GladNet;

namespace Guardians
{
	class Program
	{
		private static IGameTickable[] GameTickables { get; set; }

		private static ILog Logger { get; set; }

		private static bool isStarted { get; set; }

		static void Main(string[] args)
		{
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);
			payloadRegister.Register(VoicePayloadMetadataMarker.ClientPayloadTypesByOpcode, VoicePayloadMetadataMarker.ServerPayloadTypesByOpcode);

			Console.WriteLine("Starting test server");

			isStarted = false;
			Task.Factory.StartNew(Start);

			while(true)
				FixedUpdate();
		}

		private static async Task Start()
		{
			//TODO: Refactor this into a factory.

			ApplicationBaseContainerPair container = BuildApplicationBase();

			Logger = container.ServiceContainer.Resolve<ILog>();

			if(!container.ApplicationBase.StartServer())
			{
				string error = $"Failed to start server on Details: {container.ApplicationBase.ServerAddress.ToString()}";

				if(container.ApplicationBase.Logger.IsErrorEnabled)
					container.ApplicationBase.Logger.Error(error);

				throw new InvalidOperationException(error);
			}

			GameTickables = container.ServiceContainer
				.Resolve<IEnumerable<IGameTickable>>()
				.ToArray();

			isStarted = true;

			Logger.Info("Server begining listen.");

			try
			{
				await container.ApplicationBase.BeginListening()
					.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				Console.WriteLine($"Exception: {e.Message} \n\n Stack: {e.StackTrace} Inner: {e.InnerException?.Message} \n\n InnerStack: {e.InnerException?.StackTrace}");
				throw;
			}

			if(container.ApplicationBase.Logger.IsWarnEnabled)
				container.ApplicationBase.Logger.Warn($"Server is shutting down.");
		}

		//TODO: Create factory/clean this up
		private static ApplicationBaseContainerPair BuildApplicationBase()
		{
			DefaultZoneServerApplicationBaseFactory appBaseFactory = new DefaultZoneServerApplicationBaseFactory();

			try
			{
				return appBaseFactory.Create(new ZoneServerApplicationBaseCreationContext(new ConsoleLogger(LogLevel.All), new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 5006)));
			}
			catch(Exception e)
			{

				Console.WriteLine($"Encounter Exception: {e.Message} \n\n{e.StackTrace}\n\n");

				if(e.InnerException != null)
					Console.WriteLine($"Inner Exception: {e.InnerException.Message}\n\n{e.InnerException.StackTrace}\n\n");

				throw;
			}
		}

		static void FixedUpdate()
		{
			if(!isStarted)
				return;

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
				GameTickables[i].Tick();
		}
	}
}
