using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using Nito.AsyncEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GladMMO
{
	//This just selects the character as soon as one is clicked.
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener : BaseSingleEventListenerInitializable<ICharacterSelectionButtonClickedEventSubscribable, CharacterButtonClickedEventArgs>
	{
		//private ICharacterService CharacterService { get; }

		private ICharacterDataRepository CharacterData { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepo { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener(
			[NotNull] ICharacterSelectionButtonClickedEventSubscribable subscriptionService, 
			//[NotNull] ICharacterService characterService, 
			[NotNull] ICharacterDataRepository characterData, 
			[NotNull] IReadonlyAuthTokenRepository authTokenRepo,
			[NotNull] ILog logger) 
			: base(subscriptionService)
		{
			//CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			AuthTokenRepo = authTokenRepo ?? throw new ArgumentNullException(nameof(authTokenRepo));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, CharacterButtonClickedEventArgs args)
		{
			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				//We must actually create a session
				//before the character can login.
				/*CharacterSessionEnterResponse enterResponse = await CharacterService.TryEnterSession(args.CharacterGuid.EntityId, AuthTokenRepo.RetrieveWithType())
					.ConfigureAwait(true);

				//TODO: handle character session failure
				CharacterData.UpdateCharacterId(args.CharacterGuid.EntityId);

				if(Logger.IsInfoEnabled)
					Logger.Info($"Recieved character session entry response: {enterResponse.isSuccessful} Result: {enterResponse.ResultCode} for ZoneId: {enterResponse.ZoneId}");*/

				//TODO: Use the scene manager service.
				//TODO: Don't hardcode scene ids. Don't load scenes directly.
				SceneManager.LoadSceneAsync(2).allowSceneActivation = true;
			});
		}
	}
}
