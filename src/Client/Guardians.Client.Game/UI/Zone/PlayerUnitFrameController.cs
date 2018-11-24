using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using GaiaOnline;
using UnityEngine;
using UnityEngine.UI;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene)]
	public sealed class PlayerUnitFrameController : IGameInitializable
	{
		private IUIText PlayerNameTextField { get; }

		private IReadonlyLocalPlayerDetails LocalPlayerDetails { get; }

		private INameQueryService NameQueryable { get; }

		//TODO: We should not directly depend on these, we should access the avatar by some other abstraction
		private IGaiaOnlineQueryClient GaiaQueryClient { get; }

		//TODO: We should not directly depend on these, we should access the avatar by some other abstraction
		private IGaiaOnlineImageCDNClient GaiaImageCDNClient { get; }

		private IUIImage PlayerAvatarPortriat { get; }

		/// <inheritdoc />
		public PlayerUnitFrameController(
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIText playerNameTextField,
			IReadonlyLocalPlayerDetails localPlayerDetails,
			INameQueryService nameQueryable,
			IGaiaOnlineQueryClient gaiaQueryClient,
			IGaiaOnlineImageCDNClient gaiaImageCdnClient,
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIImage playerAvatarPortriat)
		{
			PlayerNameTextField = playerNameTextField ?? throw new ArgumentNullException(nameof(playerNameTextField));
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
			GaiaQueryClient = gaiaQueryClient ?? throw new ArgumentNullException(nameof(gaiaQueryClient));
			GaiaImageCDNClient = gaiaImageCdnClient ?? throw new ArgumentNullException(nameof(gaiaImageCdnClient));
			PlayerAvatarPortriat = playerAvatarPortriat ?? throw new ArgumentNullException(nameof(playerAvatarPortriat));
		}

		public async Task OnGameInitialized()
		{
			string nameQueryResponseValue = await NameQueryable.RetrieveAsync(LocalPlayerDetails.LocalPlayerGuid.EntityId)
				.ConfigureAwait(false);

			//Join main thread to touch Unity3D UI
			await new UnityYieldAwaitable();

			PlayerNameTextField.Text = nameQueryResponseValue;

			//TODO: We should not directly query Gaia here. We should abstract this into async dictionary we can query for loaded avatars
			UserAvatarQueryResponse avatarQueryResponse = await GaiaQueryClient.GetAvatarFromUsername(nameQueryResponseValue)
				.ConfigureAwait(true);

			if(!avatarQueryResponse.isRequestSuccessful)
				throw new InvalidOperationException($"Failed to query gaia avatar url for Name: {nameQueryResponseValue} Error: {avatarQueryResponse.ResponseStatusCode}");

			//Now we must load the avatar texture
			Texture2DWrapper texture2DWrapper = await GaiaImageCDNClient.GetAvatarImage(avatarQueryResponse.AvatarRelativeUrlPath)
				.ConfigureAwait(true);

			PlayerAvatarPortriat.SetSpriteTexture(texture2DWrapper.Texture.Value);
		}
	}
}
