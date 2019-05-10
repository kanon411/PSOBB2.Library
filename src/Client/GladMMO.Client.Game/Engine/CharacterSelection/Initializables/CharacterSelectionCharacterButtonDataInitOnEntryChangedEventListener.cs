using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac.Features.AttributeFilters;
using Glader.Essentials;
using Nito.AsyncEx;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ICharacterSelectionButtonClickedEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionCharacterButtonDataInitOnEntryChangedEventListener : BaseSingleEventListenerInitializable<ICharacterSelectionEntryDataChangeEventSubscribable, CharacterSelectionEntryDataChangeEventArgs>, ICharacterSelectionButtonClickedEventSubscribable
	{
		private INameQueryService NameQueryService { get; }

		//This state helps manage the button index.
		private int ButtonIndex = -1;

		private IReadOnlyCollection<IUILabeledButton> CharacterButtons { get; }

		/// <inheritdoc />
		public event EventHandler<CharacterButtonClickedEventArgs> OnCharacterButtonClicked;

		/// <inheritdoc />
		public CharacterSelectionCharacterButtonDataInitOnEntryChangedEventListener(
			[NotNull] ICharacterSelectionEntryDataChangeEventSubscribable subscriptionService,
			[NotNull] INameQueryService nameQueryService,
			[KeyFilter(UnityUIRegisterationKey.CharacterSelection)] [NotNull] IReadOnlyCollection<IUILabeledButton> characterButtons) 
			: base(subscriptionService)
		{
			NameQueryService = nameQueryService ?? throw new ArgumentNullException(nameof(nameQueryService));
			CharacterButtons = characterButtons ?? throw new ArgumentNullException(nameof(characterButtons));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, CharacterSelectionEntryDataChangeEventArgs args)
		{
			//At this point, we have a new character. BUT we don't know the name of it yet
			//We must query the name service for it.

			int slot = Interlocked.Increment(ref ButtonIndex);

			UnityAsyncHelper.UnityMainThreadContext.PostAsync(async () =>
			{
				//TODO: Handle errors
				//TODO: We should expose NetworkEntityGuid endpoint
				string name = await NameQueryService.RetrieveAsync(args.CharacterEntityGuid.EntityId)
					.ConfigureAwait(true);

				//Once we have the result, we can assign the name.
				IUILabeledButton button = CharacterButtons.ElementAt(slot);
				button.Text = name;
				button.IsInteractable = true;

				//When clicked just broadcast a named event to everything that it has been clicked, and who it was.
				button.AddOnClickListener(() => OnCharacterButtonClicked?.Invoke(this, new CharacterButtonClickedEventArgs(args.CharacterEntityGuid, slot)));
			});
		}
	}
}
