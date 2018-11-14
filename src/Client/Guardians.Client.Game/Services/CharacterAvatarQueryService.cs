using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GaiaOnline;
using UnityEngine;

namespace Guardians
{
	public sealed class CharacterAvatarQueryService : IAvatarTextureQueryable
	{
		private INameQueryService NameQueryable { get; }

		//TODO: We should not directly depend on these, we should access the avatar by some other abstraction
		private IGaiaOnlineQueryClient GaiaQueryClient { get; }

		//TODO: We should not directly depend on these, we should access the avatar by some other abstraction
		private IGaiaOnlineImageCDNClient GaiaImageCDNClient { get; }

		/// <inheritdoc />
		public CharacterAvatarQueryService(
			INameQueryService nameQueryable,
			IGaiaOnlineQueryClient gaiaQueryClient,
			IGaiaOnlineImageCDNClient gaiaImageCdnClient)
		{
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
			GaiaQueryClient = gaiaQueryClient ?? throw new ArgumentNullException(nameof(gaiaQueryClient));
			GaiaImageCDNClient = gaiaImageCdnClient ?? throw new ArgumentNullException(nameof(gaiaImageCdnClient));
		}

		public async Task<Texture2D> GetAvatarByCharacterId(int characterId)
		{
			string nameQueryResponseValue = await NameQueryable.RetrieveAsync(characterId)
				.ConfigureAwait(false);

			return await GetAvatarByCharacterName(nameQueryResponseValue);
		}

		public async Task<Texture2D> GetAvatarByCharacterName(string characterName)
		{
			UserAvatarQueryResponse avatarQueryResponse = await GaiaQueryClient.GetAvatarFromUsername(characterName)
				.ConfigureAwait(true);

			if(!avatarQueryResponse.isRequestSuccessful)
				throw new InvalidOperationException($"Failed to query gaia avatar url for Name: {characterName} Error: {avatarQueryResponse.ResponseStatusCode}");

			//Now we must load the avatar texture
			Texture2DWrapper texture2DWrapper = await GaiaImageCDNClient.GetAvatarImage(avatarQueryResponse.AvatarRelativeUrlPath)
				.ConfigureAwait(true);

			//Must be on the main thread to access the texture2D
			await new UnityYieldAwaitable();

			return texture2DWrapper.Texture.Value;
		}
	}
}
