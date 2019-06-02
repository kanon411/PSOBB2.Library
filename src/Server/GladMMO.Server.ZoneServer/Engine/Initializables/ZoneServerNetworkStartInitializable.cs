﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glader.Essentials;
using GladNet;

namespace GladMMO
{
	//Initializable that just starts the zoneserver network listener.
	[ServerSceneTypeCreate(ServerSceneType.Default)]
	public sealed class ZoneServerNetworkStartInitializable : IGameInitializable
	{
		private ZoneServerApplicationBase ApplicationBase { get; }

		/// <inheritdoc />
		public ZoneServerNetworkStartInitializable([NotNull] ZoneServerApplicationBase applicationBase)
		{
			ApplicationBase = applicationBase ?? throw new ArgumentNullException(nameof(applicationBase));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			if(!ApplicationBase.StartServer())
			{
				string error = $"Failed to start server on Details: {ApplicationBase.ServerAddress}";

				if(ApplicationBase.Logger.IsErrorEnabled)
					ApplicationBase.Logger.Error(error);

				throw new InvalidOperationException(error);
			}

			Task.Factory.StartNew(async () =>
			{
				await ApplicationBase.BeginListening()
					.ConfigureAwait(false);

				if(ApplicationBase.Logger.IsWarnEnabled)
					ApplicationBase.Logger.Warn($"Server is shutting down.");

			}, TaskCreationOptions.LongRunning);

			return Task.CompletedTask;
		}
	}
}
