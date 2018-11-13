using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.CharacterSelection)]
	public sealed class CharacterSelectionListInitializer : IGameInitializable
	{
		private ICharacterService CharacterQueryable { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepository { get; }

		private ILog Logger { get; }

		private IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext> CharacterSlotFactory { get; }

		/// <inheritdoc />
		public CharacterSelectionListInitializer(
			ICharacterService characterQueryable, 
			IReadonlyAuthTokenRepository authTokenRepository, 
			ILog logger, 
			IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext> characterSlotFactory)
		{
			CharacterQueryable = characterQueryable ?? throw new ArgumentNullException(nameof(characterQueryable));
			AuthTokenRepository = authTokenRepository ?? throw new ArgumentNullException(nameof(authTokenRepository));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterSlotFactory = characterSlotFactory;
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
			}
		}
	}
}
