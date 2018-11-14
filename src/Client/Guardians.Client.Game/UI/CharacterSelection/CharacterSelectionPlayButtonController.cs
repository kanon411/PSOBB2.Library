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

		/// <inheritdoc />
		public CharacterSelectionPlayButtonController(
			[KeyFilter(UnityUIRegisterationKey.Login)] IUIButton playButton,
			ISceneManager sceneManagementService)
		{
			PlayButton = playButton ?? throw new ArgumentNullException(nameof(playButton));
			SceneManagementService = sceneManagementService ?? throw new ArgumentNullException(nameof(sceneManagementService));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			await new UnityYieldAwaitable();

			//TODO: In the future we need to connect to the GameServer and try to create a session. If that suceeds we'll know what map to load too.
			PlayButton.AddOnClickListener(() =>
			{
				//Prevent multiple play interactions.
				PlayButton.IsInteractable = false;
				SceneManagementService.LoadLevel((int)GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene, LoadSceneMode.Single);
			});
		}
	}
}
