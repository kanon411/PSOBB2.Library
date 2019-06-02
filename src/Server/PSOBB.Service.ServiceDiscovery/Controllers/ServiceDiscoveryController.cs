﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PSOBB
{
	/// <summary>
	/// The service discovery controller.
	/// Endpoints/actions are documented in the documentation repo.
	/// </summary>
	[Route("api/[controller]")]
	public sealed class ServiceDiscoveryController : Controller
	{
		/// <summary>
		/// The endpoint repository.
		/// </summary>
		private IRegionbasedNameEndpointResolutionRepository EndpointRepository { get; }

		public ILogger<ServiceDiscoveryController> LoggingService { get; }

		public ServiceDiscoveryController([FromServices] IRegionbasedNameEndpointResolutionRepository endpointRepository, ILogger<ServiceDiscoveryController> loggingService)
		{
			if (endpointRepository == null) throw new ArgumentNullException(nameof(endpointRepository));
			if (loggingService == null) throw new ArgumentNullException(nameof(loggingService));

			EndpointRepository = endpointRepository;
			LoggingService = loggingService;
		}

		[HttpGet(nameof(Discover))]
		public Task<ResolveServiceEndpointResponse> Discover([FromQuery] string serviceName)
		{
			return Discover(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceName));
		}

		[HttpPost(nameof(Discover))]
		public async Task<ResolveServiceEndpointResponse> Discover([FromBody] ResolveServiceEndpointRequest requestModel)
		{
			if(LoggingService.IsEnabled(LogLevel.Debug))
				LoggingService.LogDebug($"Service Discover request for: {requestModel.Region}:{requestModel.ServiceType}");

			if (!ModelState.IsValid)
			{
				if (LoggingService.IsEnabled(LogLevel.Debug))
					LoggingService.LogDebug($"Resolution request was sent with an invalid model ModelState.");

				return new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.GeneralRequestError);
			}

			//We need to check if we know about the locale
			//If we don't we should indicate it is unlisted
			//We also need to check if the keypair region and servicetype exist
			if (!await EndpointRepository.HasEntryAsync(requestModel.Region, requestModel.ServiceType))
			{
				if(LoggingService.IsEnabled(LogLevel.Debug))
					LoggingService.LogDebug($"Client requested unlisted service Region: {requestModel.Region} Service: {requestModel.ServiceType}.");

				return new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.ServiceUnlisted);
			}

			ResolvedEndpoint endpoint = await EndpointRepository.RetrieveAsync(requestModel.Region, requestModel.ServiceType);

			if (endpoint == null)
			{
				//Log the error. It shouldn't be null if the checks passed
				if(LoggingService.IsEnabled(LogLevel.Error))
					LoggingService.LogError($"Resolution request {requestModel.ServiceType} for region {requestModel.Region} failed even through it was a known pair.");

				return new ResolveServiceEndpointResponse(ResolveServiceEndpointResponseCode.GeneralRequestError);
			}

			//Just return the JSON model response to the client
			return new ResolveServiceEndpointResponse(endpoint);
		}
	}
}
