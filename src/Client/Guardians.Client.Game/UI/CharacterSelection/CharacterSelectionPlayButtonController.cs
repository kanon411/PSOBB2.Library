using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.CharacterSelection)]
	public sealed class CharacterSelectionPlayButtonController : IGameInitializable
	{
		private IUIButton PlayButton { get; }

		private ISceneManager SceneManagementService { get; }

		private ICharacterService CharacterService { get; }

		private ICharacterDataRepository CharacterData { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepo { get; }

		/// <inheritdoc />
		public CharacterSelectionPlayButtonController(
			[KeyFilter(UnityUIRegisterationKey.Login)] IUIButton playButton,
			ISceneManager sceneManagementService,
			ICharacterService characterService,
			ICharacterDataRepository characterData,
			IReadonlyAuthTokenRepository authTokenRepo)
		{
			PlayButton = playButton ?? throw new ArgumentNullException(nameof(playButton));
			SceneManagementService = sceneManagementService ?? throw new ArgumentNullException(nameof(sceneManagementService));
			CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			AuthTokenRepo = authTokenRepo ?? throw new ArgumentNullException(nameof(authTokenRepo));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			await new UnityYieldAwaitable();

			PlayButton.AddOnClickListenerAsync(async () =>
			{
				//We must actually create a session
				//before the character can login.

				CharacterSessionEnterResponse enterResponse = await CharacterService.TryEnterSession(CharacterData.CharacterId, AuthTokenRepo.RetrieveWithType())
					.ConfigureAwait(false);

				//TODO: handle character session failure

				//Must be on main thread to do UI/Scene loading
				await new UnityYieldAwaitable();

				//Prevent multiple play interactions.
				PlayButton.IsInteractable = false;
				//Should go to the world downloading screen
				SceneManagementService.LoadLevel((int)GameInitializableSceneSpecificationAttribute.SceneType.WorldDownloadingScreen, LoadSceneMode.Single);

			});
		}
	}
}
