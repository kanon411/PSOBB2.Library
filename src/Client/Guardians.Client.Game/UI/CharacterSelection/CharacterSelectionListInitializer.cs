using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using UnityEngine;

namespace Guardians
{
	//TODO: Refactor this, it's ABSOLUTELY HUGE and messy.
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.CharacterSelection)]
	public sealed class CharacterSelectionListInitializer : IGameInitializable
	{
		private ICharacterService CharacterQueryable { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

		private ILog Logger { get; }

		private IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext> CharacterSlotFactory { get; }

		private IUIImage AvatarDisplay { get; }

		private ICharacterDataRepository CharacterRepository { get; }

		/// <inheritdoc />
		public CharacterSelectionListInitializer(
			ICharacterService characterQueryable, 
			IReadonlyAuthTokenRepository authTokenRepository, 
			ILog logger, 
			IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext> characterSlotFactory,
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIImage avatarDisplay,
			ICharacterDataRepository characterRepository)
		{
			CharacterQueryable = characterQueryable ?? throw new ArgumentNullException(nameof(characterQueryable));
			AuthTokenRepository = authTokenRepository ?? throw new ArgumentNullException(nameof(authTokenRepository));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterSlotFactory = characterSlotFactory;
			AvatarDisplay = avatarDisplay;
			CharacterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			CharacterListResponse listResponse = await CharacterQueryable.GetCharacters(AuthTokenRepository.RetrieveWithType())
				.ConfigureAwait(false);

			if(!listResponse.isSuccessful || listResponse.CharacterIds.Count == 0)
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to query character list. Recieved ResultCode: {listResponse.ResultCode}");

			//We must be on main thread to do this
			await new UnityYieldAwaitable();

			//Foreach character we recieved we need to create a slot for it on the character list
			//the factory takes care of the heavy lifting for us.
			foreach(var character in listResponse.CharacterIds)
			{
				await CreateSlotForCharacter(character)
					.ConfigureAwait(true); //stay on main thread
			}
		}

		private async Task CreateSlotForCharacter(int character)
		{
			CharacterSlotUIElements elements = CharacterSlotFactory.Create(EmptyFactoryContext.Instance);

			//Once the actual UI element is created we need to initialize the name and callbacks
			NameQueryResponse nameQueryResponse = await CharacterQueryable.NameQuery(character)
				.ConfigureAwait(true);

			if(!nameQueryResponse.isSuccessful)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to Query Name for CharacterId: {character}. ResultCode: {nameQueryResponse.ResultCode}");

				elements.CharacterNameText.Text = "Unknown";
			}
			else
			{
				elements.CharacterNameText.Text = nameQueryResponse.EntityName;
			}

			//When a slot is created we need to disable the Toggle (assuming it's maybe on)
			//Then we need prepare the button click callbacks.
			elements.CharacterSlotToggle.AddOnToggleChangedListenerAsync(async toggled =>
			{
				//We only want to emulate the click if it was toggled ON
				if(toggled)
					await OnCharacterSlotButtonClickedAsync(character);
			});
		}

		private async Task OnCharacterSlotButtonClickedAsync(int characterId)
		{
			//TODO: Disable toggle
			//When the slot button is clicked we need to initialize the avatar display of the one selected
			//then we need to disable the toggle of all BUT this slot and
			//set the current state to the character id.

			await new UnityYieldAwaitable();

			//We just set the avatar display
			//Just set the new character id so anything else can make decisions based on it.
			CharacterRepository.UpdateCharacterId(characterId);
		}
	}
}
