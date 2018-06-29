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

		void Start()
		{
			//We need to query for the characters
			//at the start of the scene.
			CharacterService.GetCharacters(AuthTokenRepository.Retrieve())
				.UnityAsyncContinueWith(this, OnRecievedCharacterQueryResponse);
		}

		private async Task OnRecievedCharacterQueryResponse(CharacterListResponse response)
		{
			if(!response.isSuccessful)
				throw new InvalidOperationException($"Failed to query for characters. Error: {response.ResultCode}");

			//TODO: Batch request endpoint
			//We need the character names for each one
			foreach(int characterId in response.CharacterIds)
			{
				CharacterNameQueryResponse nameQueryResponse = await CharacterService.NameQuery(characterId)
					.ConfigureAwait(false);

				if(!nameQueryResponse.isSuccessful)
					throw new InvalidOperationException($"Failed to query character name for Id: {characterId} ResultCode: {nameQueryResponse.ResultCode}");

				await Task.Yield();

				//TODO: Callback
				View.SetCharacterSlot(nameQueryResponse.CharacterName, () => Debug.Log($"Pressed Id: {characterId}"));
			}
		}
	}
}
