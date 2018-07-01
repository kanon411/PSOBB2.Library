using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GaiaOnline;
using SceneJect.Common;
using UnityEngine;
using UnityEngine.Events;
using Unitysync.Async;

namespace Guardians
{
	[Injectee]
	public sealed class CharacterSelectionUIController : UIController<CharacterSelectionView>
	{
		[Serializable]
		private sealed class CharacterSessionCreatedEvent : UnityEvent<int> { }

		[Inject]
		private ICharacterService CharacterService { get; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

		[Inject]
		private INameQueryService NameQueryService { get; }

		[Inject]
		private IGaiaOnlineQueryClient GaiaQueryClient { get; }

		[Inject]
		private IGaiaOnlineImageCDNClient GaiaImageCDNClient { get; }

		[Inject]
		private ICharacterDataRepository CharacterRepository { get; }

		[SerializeField]
		private CharacterSessionCreatedEvent OnCharacterSessionCreated;

		private void Start()
		{
			//We need to query for the characters
			//at the start of the scene.
			CharacterService.GetCharacters(AuthTokenRepository.RetrieveWithType())
				.UnityAsyncContinueWith(this, OnRecievedCharacterQueryResponse);
		}

		private async Task OnRecievedCharacterQueryResponse(CharacterListResponse response)
		{
			if(!response.isSuccessful)
				throw new InvalidOperationException($"Failed to query for characters. Error: {response.ResultCode}");

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Recieved query response with Code:{response.ResultCode} CharacterCount: {response.CharacterIds.Count}");

			//TODO: Batch request endpoint
			//We need the character names for each one
			foreach(int characterId in response.CharacterIds)
			{
				string characterName = await NameQueryService.RetrieveAsync(characterId)
					.ConfigureAwait(true);

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Recieved characterId: {characterId}");

				//TODO: Callback
				View.SetCharacterSlot(characterName, () => OnCharacterButtonClicked(characterId));
			}
		}

		public void OnEnterWorldButtonClicked()
		{
			//We should do nothing if the characterId hasn't been set
			if(CharacterRepository.CharacterId == default(int))
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug("Cannot enter the world. No ID is set.");

				return;
			}

			//Otherwise we can log in. Meaning we need to create a session on the server
			CharacterService.TryEnterSession(CharacterRepository.CharacterId, AuthTokenRepository.RetrieveWithType())
				.UnityAsyncContinueWith(this, OnSessionEnterResult);
		}

		private async Task OnSessionEnterResult(CharacterSessionEnterResponse result)
		{
			if(!result.isSuccessful)
			{
				string error = $"Failed to enter session for Id: {CharacterRepository.CharacterId} Error: {result.ResultCode}";

				if(Logger.IsErrorEnabled)
					Logger.Error(error);

				ErrorView.SetError(error);
				return;
			}

			//So, the session has been created successfully
			//This means we should move to the actual zone scene that we should be in
			//We will let some other object handle that responsibility and we will just announce
			//that we have selected a character.

			//Don't bother with zoneid or zone type stuff right now
			//it is not fully implemented
			OnCharacterSessionCreated?.Invoke(0);
		}

		public void OnCharacterButtonClicked(int characterId)
		{
			if(characterId < 0) throw new ArgumentOutOfRangeException(nameof(characterId));

			CharacterRepository.UpdateCharacterId(characterId);

			//TODO: Should we use the async?
			NameQueryService.RetrieveAsync(characterId)
				.UnityAsyncContinueWith(this, OnCharacterButtonClickedAsync);
		}

		private async Task OnCharacterButtonClickedAsync(string characterName)
		{
			if(string.IsNullOrWhiteSpace(characterName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(characterName));

			UserAvatarQueryResponse avatarQueryResponse = await GaiaQueryClient.GetAvatarFromUsername(characterName)
				.ConfigureAwait(true);

			if(!avatarQueryResponse.isRequestSuccessful)
				throw new InvalidOperationException($"Failed to query gaia avatar url for Name: {characterName} Error: {avatarQueryResponse.ResponseStatusCode}");

			//Now we must load the avatar texture
			Texture2DWrapper texture2DWrapper = await GaiaImageCDNClient.GetAvatarImage(avatarQueryResponse.AvatarRelativeUrlPath)
				.ConfigureAwait(true);

			View.SetCharacterPreview(texture2DWrapper.Texture.Value);
		}
	}
}
