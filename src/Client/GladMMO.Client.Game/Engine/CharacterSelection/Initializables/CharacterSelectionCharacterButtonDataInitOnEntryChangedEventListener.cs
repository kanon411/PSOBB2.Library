using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using Glader.Essentials;
using Nito.AsyncEx;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(ICharacterSelectionButtonClickedEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionCharacterButtonDataInitOnEntryChangedEventListener : EventQueueBasedTickable<ICharacterSelectionEntryDataChangeEventSubscribable, CharacterSelectionEntryDataChangeEventArgs>, ICharacterSelectionButtonClickedEventSubscribable
	{
		//This state helps manage the button index.
		private int ButtonIndex = -1;

		private IReadOnlyCollection<IUICharacterSlot> CharacterButtons { get; }

		/// <inheritdoc />
		public event EventHandler<CharacterButtonClickedEventArgs> OnCharacterButtonClicked;

		/// <inheritdoc />
		public CharacterSelectionCharacterButtonDataInitOnEntryChangedEventListener(ILog logger,
			[NotNull] ICharacterSelectionEntryDataChangeEventSubscribable subscriptionService,
			[KeyFilter(UnityUIRegisterationKey.CharacterSelection)] [NotNull] IReadOnlyCollection<IUICharacterSlot> characterButtons)
			: base(subscriptionService, false, logger)
		{
			CharacterButtons = characterButtons ?? throw new ArgumentNullException(nameof(characterButtons));
		}

		/// <inheritdoc />
		protected override void HandleEvent(CharacterSelectionEntryDataChangeEventArgs args)
		{
			//TODO: This should run on main thread now, no need to interlocked
			//At this point, we have a new character. BUT we don't know the name of it yet
			//We must query the name service for it.
			int slot = Interlocked.Increment(ref ButtonIndex);

			//Once we have the result, we can assign the name.
			IUICharacterSlot button = CharacterButtons.ElementAt(slot);
			button.Text = "TODO IMPLEMENT NAMES AGAIN";
			button.IsInteractable = true;
			button.SetElementActive(true);

			//When clicked just broadcast a named event to everything that it has been clicked, and who it was.
			button.AddOnToggleChangedListener(toggleState =>
			{
				if(toggleState)
					OnCharacterButtonClicked?.Invoke(this, new CharacterButtonClickedEventArgs(args.CharacterEntityGuid, slot));
			});
		}
	}
}
