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

		[Inject]
		private INameQueryService NameQueryService { get; }

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

		public void OnCharacterButtonClicked(int characterId)
		{
			//TODO: Should we use the async?
			string characterName = NameQueryService.Retrieve(characterId);

			if(Logger.IsDebugEnabled)
				Logger.Debug($"Clicked: {characterName}");
		}
	}
}
