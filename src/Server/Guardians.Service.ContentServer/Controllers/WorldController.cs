using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Guardians
{
	//From ProjectVindictive: https://github.com/HelloKitty/ProjectVindictive.Library/blob/master/src/ProjectVindictive.Service.UserContentManagement/Controllers/WorldController.cs
	[Route("api/[controller]")]
	public class WorldController : AuthorizationReadyController
	{
		/// <summary>
		/// Logging serivce.
		/// </summary>
		private ILogger<WorldController> Logger { get; }

		// GET api/values
		/// <inheritdoc />
		public WorldController(IClaimsPrincipalReader claimsReader, ILogger<WorldController> logger)
			: base(claimsReader, logger)
		{
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;
		}

		/// <summary>
		/// POST request that requests an upload URL for a world.
		/// The user must be authorized.
		/// </summary>
		/// <returns>A <see cref="RequestedUrlResponseModel"/> that either contains error information or the upload URL if it was successful.</returns>
		[HttpPost("create")]
		[AuthorizeJwt]
		public async Task<IActionResult> RequestWorldUploadUrl([FromServices] IWorldEntryRepository worldEntryRepository, [FromServices] IStorageUrlBuilder urlBuilder)
		{
			if(worldEntryRepository == null) throw new ArgumentNullException(nameof(worldEntryRepository));

			//TODO: We want to rate limit access to this API
			//TODO: We should use both app logging but also another logging service that always gets hit

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Recieved {nameof(RequestWorldUploadUrl)} request from {ClaimsReader.GetUserName(User)}:{ClaimsReader.GetUserId(User)}.");

			int userId = ClaimsReader.GetUserIdInt(User);

			//TODO: We should send this if we can't get a user id
			//return new JsonResult(RequestedUrlResponseModel.CreateFailure("Failed to authorize action.", RequestedUrlResponseCode.AuthorizationFailed));
			//TODO: Abstract this behind an issuer
			Guid worldGuid = Guid.NewGuid();

			//TODO: Check if the result is valid? We should maybe return bool from this API
			//The idea is to create an entry which will contain a GUID. From that GUID we can then generate the upload URL
			bool result = await worldEntryRepository.TryCreateAsync(new WorldEntryModel(userId, this.HttpContext.Connection.RemoteIpAddress.ToString(), worldGuid)); //TODO: Ok to just provide a guid right?

			string uploadUrl = await urlBuilder.BuildUploadUrl(UserContentType.World, worldGuid);

			if(String.IsNullOrEmpty(uploadUrl))
			{
				if(Logger.IsEnabled(LogLevel.Error))
					Logger.LogError($"Failed to create world upload URL for {ClaimsReader.GetUserName(User)}:{ClaimsReader.GetUserId(User)} with GUID: {worldGuid}.");

				return new JsonResult(RequestedUrlResponseModel.CreateFailure("Upload service unavailable.", RequestedUrlResponseCode.ServiceUnavailable));
			}

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Success. Sending {ClaimsReader.GetUserName(User)} URL: {uploadUrl}");

			return new JsonResult(RequestedUrlResponseModel.CreateSuccess(uploadUrl));
		}
	}
}