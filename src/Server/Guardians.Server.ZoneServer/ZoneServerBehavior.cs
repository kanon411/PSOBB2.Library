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
		void Awake()
		{
			RuntimeTypeModel.Default.Add(typeof(GameClientPacketPayload), true);
			RuntimeTypeModel.Default.Add(typeof(GameServerPacketPayload), true);

			ZoneServerMetadataMarker.ClientPayloadTypesByOpcode
				.AsEnumerable()
				.Concat(ZoneServerMetadataMarker.ServerPayloadTypesByOpcode)
				.ToList()
				.ForEach(pair =>
				{
					//TODO: If they don't have the the direct base-type matching this will cause exceptions
					RuntimeTypeModel.Default.Add(pair.Value, true);

					RuntimeTypeModel.Default[pair.Value.BaseType]
						.AddSubType((int)pair.Key, pair.Value);

				});
		}

		private async Task Start()
		{
			//TODO: Refactor this into a factory.

			ApplicationBaseContainerPair container = BuildApplicationBase();

			if(!container.ApplicationBase.StartServer())
			{
				string error = $"Failed to start server on Details: {container.ApplicationBase.ServerAddress}";

				if(container.ApplicationBase.Logger.IsErrorEnabled)
					container.ApplicationBase.Logger.Error(error);

				throw new InvalidOperationException(error);
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
	}
}
