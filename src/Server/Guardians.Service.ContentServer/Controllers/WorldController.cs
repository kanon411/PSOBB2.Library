using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;
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

		[HttpPost("{id}/uploaded")]
		[AuthorizeJwt]
		public async Task<IActionResult> SetWorldAsUploaded([FromRoute(Name = "id")] long worldId, [FromServices] IWorldEntryRepository worldEntryRepository, [FromServices] IAmazonS3 storageClient)
		{
			//At this point, the user is telling us they finished uploading the world.
			//They could be lying so we should check that the resource exists AND
			//we should also check that it's an asset bundle and gather some information from the header.

			//First we verify a world exists with this id
			if(!await worldEntryRepository.ContainsAsync(worldId).ConfigureAwait(false))
			{
				//TODO: We should say something more specific
				return BadRequest();
			}

			WorldEntryModel model = await worldEntryRepository.RetrieveAsync(worldId)
				.ConfigureAwait(false);

			//Check the model is associated with this account. Only 1 account can own a world resource
			if(model.AccountId != ClaimsReader.GetUserIdInt(User))
				return Unauthorized();

			//Now that we know the world is in the database and the account making this authorized requests owns it
			//we can now actually check that the resource exists on the storeage system
			//TODO: This relies on some outdated API/deprecated stuff.
			bool resourceExists = await S3ResourceExists(storageClient, "projectvindictive-dev", (await Amazon.Util.ProfileManager.GetAWSCredentials("projectvindictive-dev").GetCredentialsAsync().ConfigureAwait(false)).AccessKey)
				.ConfigureAwait(false); //TODO: Don't hardcore bucket name

			//TODO: Be more descriptive
			if(!resourceExists)
				return NotFound();

			//Ok, so the user IS the resource owner AND he did upload something, so let's validate the assetbundle header.
			//TODO: Refactor this into an object that does the validation and generates readable data
			//TODO: Actually implement asset bundle validation
			//We haven't implemented this yet, we should do asset bundle parsing and validation
			//This REALLY important to prevent invalid bundles from being uploaded
			//or content that isn't even an asset bundle being uploaded
			//See: https://github.com/HearthSim/UnityPack/wiki/Format-Documentation

			//For now, since it's unimplemented let's just set it validated
			await worldEntryRepository.SetWorldValidated(model.WorldId)
				.ConfigureAwait(false);

			return Ok();
		}

		private async Task<bool> S3ResourceExists(IAmazonS3 client, string bucket, string key)
		{
			//This is actually how the old AWS client worked: https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/S3/Custom/_bcl/IO/S3FileInfo.cs
			//Kinda bad design tbh Amazon lol
			try
			{
				var request = new GetObjectMetadataRequest
				{
					BucketName = bucket,
					Key = key.Replace('\\', '/') //S3helper.EncodeKey: https://github.com/aws/aws-sdk-net/blob/b691e46e57a3e24477e6a5fa2e849da44db7002f/sdk/src/Services/S3/Custom/_bcl/IO/S3Helper.cs
				};
				((Amazon.Runtime.Internal.IAmazonWebServiceRequest)request).AddBeforeRequestHandler(FileIORequestEventHandler);

				// If the object doesn't exist then a "NotFound" will be thrown
				await client.GetObjectMetadataAsync(request)
					.ConfigureAwait(false);

				return true;
			}
			catch(AmazonS3Exception e)
			{
				return false;
			}
		}

		//From: https://github.com/aws/aws-sdk-net/blob/b691e46e57a3e24477e6a5fa2e849da44db7002f/sdk/src/Services/S3/Custom/_bcl/IO/S3Helper.cs
		internal static void FileIORequestEventHandler(object sender, RequestEventArgs args)
		{
			WebServiceRequestEventArgs wsArgs = args as WebServiceRequestEventArgs;
			if(wsArgs != null)
			{
				string currentUserAgent = wsArgs.Headers[AWSSDKUtils.UserAgentHeader];
				wsArgs.Headers[AWSSDKUtils.UserAgentHeader] = currentUserAgent + " FileIO";
			}
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

			//TODO: Check if the result is valid? We should maybe return bool from this API (we do return bool from this API now)
			//The idea is to create an entry which will contain a GUID. From that GUID we can then generate the upload URL
			WorldEntryModel world = new WorldEntryModel(userId, this.HttpContext.Connection.RemoteIpAddress.ToString(), worldGuid);
			bool result = await worldEntryRepository.TryCreateAsync(world); //TODO: Ok to just provide a guid right?

			//TODO: Check world's worldid has been set

			string uploadUrl = await urlBuilder.BuildUploadUrl(UserContentType.World, worldGuid);

			if(String.IsNullOrEmpty(uploadUrl))
			{
				if(Logger.IsEnabled(LogLevel.Error))
					Logger.LogError($"Failed to create world upload URL for {ClaimsReader.GetUserName(User)}:{ClaimsReader.GetUserId(User)} with GUID: {worldGuid}.");

				return new JsonResult(RequestedUrlResponseModel.CreateFailure("Upload service unavailable.", RequestedUrlResponseCode.ServiceUnavailable));
			}

			if(Logger.IsEnabled(LogLevel.Information))
				Logger.LogInformation($"Success. Sending {ClaimsReader.GetUserName(User)} URL: {uploadUrl}");

			return new JsonResult(RequestedUrlResponseModel.CreateSuccess(uploadUrl, world.WorldId));
		}
	}
}