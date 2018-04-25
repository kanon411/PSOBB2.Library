using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HaloLive.Models.NameResolution;
using HaloLive.Network.Common;

namespace Guardians
{
	/// <summary>
	/// Loads Endpoint{Region}.json files from the Endpoints directory.
	/// This directory should be relative to the working directory of the launched application.
	/// </summary>
	public sealed class FilestoreBasedRegionNamedEndpointStoreRepository : IRegionNamedEndpointStoreRepository
	{
		/// <inheritdoc />
		public async Task<NameEndpointResolutionStorageModel> Retrieve(ClientRegionLocale region)
		{
			if (!Enum.IsDefined(typeof(ClientRegionLocale), region)) throw new ArgumentOutOfRangeException(nameof(region), "Value should be defined in the ClientRegionLocale enum.");

			using (StreamReader reader = File.OpenText(BuildRegionEndpointFileLocation(region)))
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<NameEndpointResolutionStorageModel>(await reader.ReadToEndAsync());
			}
		}

		/// <inheritdoc />
		public Task<bool> HasRegionStore(ClientRegionLocale region)
		{
			if (!Enum.IsDefined(typeof(ClientRegionLocale), region)) throw new ArgumentOutOfRangeException(nameof(region), "Value should be defined in the ClientRegionLocale enum.");

			return Task.FromResult(File.Exists(BuildRegionEndpointFileLocation(region)));
		}

		private string BuildRegionEndpointFileLocation(ClientRegionLocale region)
		{
			if (!Enum.IsDefined(typeof(ClientRegionLocale), region)) throw new ArgumentOutOfRangeException(nameof(region), "Value should be defined in the ClientRegionLocale enum.");

			return $@"Endpoints/Endpoints{region.ToString()}.json";
		}
	}
}
