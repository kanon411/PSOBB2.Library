using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SceneJect.Common;
using UnityEngine;
using Unitysync.Async;

namespace Guardians
{
	[Injectee]
	public sealed class CharacterSelectionUIController : UIController<CharacterSelectionView>
	{
		[Inject]
		private ICharacterService CharacterService { get; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

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
				CharacterNameQueryResponse nameQueryResponse = await CharacterService.NameQuery(characterId)
					.ConfigureAwait(false);

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Recieved name query response with Code:{nameQueryResponse.ResultCode} Name: {nameQueryResponse.CharacterName}");

				if(!nameQueryResponse.isSuccessful)
					throw new InvalidOperationException($"Failed to query character name for Id: {characterId} ResultCode: {nameQueryResponse.ResultCode}");

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Before yield.");

				await Task.Yield();

				if(Logger.IsDebugEnabled)
					Logger.Debug($"after yield.");

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Recieved characterId: {characterId}");

				//TODO: Callback
				View.SetCharacterSlot(nameQueryResponse.CharacterName, () => Logger.Debug($"Pressed Id: {characterId}"));
			}
		}
	}
}
