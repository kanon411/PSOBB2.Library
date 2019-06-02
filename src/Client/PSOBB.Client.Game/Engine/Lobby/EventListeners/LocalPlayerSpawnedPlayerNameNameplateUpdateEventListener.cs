using System;
using System.Collections.Generic;
using System.Text;
using Autofac.Features.AttributeFilters;
using Nito.AsyncEx;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class LocalPlayerSpawnedPlayerNameNameplateUpdateEventListener : BaseSingleEventListenerInitializable<ILocalPlayerSpawnedEventSubscribable, LocalPlayerSpawnedEventArgs>
	{
		private IUIText PlayerNameTextField { get; }

		private INameQueryService NameQueryable { get; }

			/// <inheritdoc />
		public LocalPlayerSpawnedPlayerNameNameplateUpdateEventListener([NotNull] ILocalPlayerSpawnedEventSubscribable subscriptionService,
				[NotNull] [KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIText playerNameTextField,
				[NotNull] INameQueryService nameQueryable) 
				: base(subscriptionService)
		{
			if(subscriptionService == null) throw new ArgumentNullException(nameof(subscriptionService));
			PlayerNameTextField = playerNameTextField ?? throw new ArgumentNullException(nameof(playerNameTextField));
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, LocalPlayerSpawnedEventArgs args)
		{
			//TODO: Find a better way to do async stuff on events.
			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				string nameQueryResponseValue = await NameQueryable.RetrieveAsync(args.EntityGuid.EntityId)
					.ConfigureAwait(true);

				PlayerNameTextField.Text = nameQueryResponseValue;
			});
		}
	}
}
