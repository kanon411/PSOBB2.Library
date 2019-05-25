using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;
using GladNet;
using Nito.AsyncEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GladMMO
{
	//This just selects the character as soon as one is clicked.
	[AdditionalRegisterationAs(typeof(IServerRequestedSceneChangeEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener : BaseSingleEventListenerInitializable<ICharacterSelectionButtonClickedEventSubscribable, CharacterButtonClickedEventArgs>, IServerRequestedSceneChangeEventSubscribable
	{
		private ICharacterDataRepository CharacterData { get; }

		private ILog Logger { get; }

		private IPeerPayloadSendService<GamePacketPayload> SendService { get; }

		private IUIButton EnterWorldButton { get; }

		/// <inheritdoc />
		public event EventHandler<ServerRequestedSceneChangeEventArgs> OnServerRequestedSceneChange;

		private NetworkEntityGuid SelectedCharacterGuid { get; set; }

		/// <inheritdoc />
		public CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener(
			[NotNull] ICharacterSelectionButtonClickedEventSubscribable subscriptionService, 
			[NotNull] ICharacterDataRepository characterData, 
			[NotNull] ILog logger,
			[NotNull] IPeerPayloadSendService<GamePacketPayload> sendService,
			[KeyFilter(UnityUIRegisterationKey.EnterWorld)] [NotNull] IUIButton enterWorldButton)
			: base(subscriptionService)
		{
			//CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
			EnterWorldButton = enterWorldButton ?? throw new ArgumentNullException(nameof(enterWorldButton));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, CharacterButtonClickedEventArgs args)
		{
			SelectedCharacterGuid = args.CharacterGuid;
		}

		/// <inheritdoc />
		public override Task OnGameInitialized()
		{
			base.OnGameInitialized();

			EnterWorldButton.IsInteractable = false;
			EnterWorldButton.AddOnClickListener(() => EnterWorldButton.IsInteractable = false);
			EnterWorldButton.AddOnClickListenerAsync(OnEnterWorldButtonClicked);

			return Task.CompletedTask;
		}

		private async Task OnEnterWorldButtonClicked()
		{
			if(SelectedCharacterGuid == null)
			{
				Logger.Error($"Tried to enter the world without any selected character guid.");
				return;
			}
				

			//We do this before sending the player login BECAUSE of a race condition that can be caused
			//since I actually KNOW this event should disable networking. We should not handle messages in this scene after this point basically.
			//TODO: Don't hardcode this scene.
			OnServerRequestedSceneChange?.Invoke(this, new ServerRequestedSceneChangeEventArgs((PlayableGameScene)2));

			//So, this just tells TrinityCore that we want to login into THIS character.
			//So, it'll attempt to do so.
			//There isn't really a response to this, TrinityCore will just send back: SMSG_LOGIN_VERIFY_WORLD
			//when it's successful and then the player will begin actually creating itself in the world and recieving
			//some data about the world state.
			throw new NotImplementedException($"TODO: Reimplement new CharacterLoginRequest(SelectedCharacterGuid)");
			//await SendService.SendMessage(new CharacterLoginRequest(SelectedCharacterGuid));

			//TODO: handle character session failure
			CharacterData.UpdateCharacterId(SelectedCharacterGuid.EntityId);

			//TODO: Use the scene manager service.
			//TODO: Don't hardcode scene ids. Don't load scenes directly.
			SceneManager.LoadSceneAsync(2).allowSceneActivation = true;
		}
	}
}
