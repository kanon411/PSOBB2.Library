using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene)]
	public sealed class PlayerUnitFameResourceController : IGameInitializable
	{
		/// <summary>
		/// The UI text that represents the player's health percentage.
		/// </summary>
		private IUIText PlayerHealthPercentageText { get; }

		/// <summary>
		/// The UI health bar of the local player.
		/// </summary>
		private IUIFillableImage PlayerHealthBar { get; }

		/// <summary>
		/// The local player's details
		/// </summary>
		private IReadonlyLocalPlayerDetails PlayerDetails { get; }

		private IEntityDataChangeCallbackRegisterable EntityDataCallbackRegister { get; }

		/// <inheritdoc />
		public PlayerUnitFameResourceController(
			[KeyFilter(UnityUIRegisterationKey.PlayerHealthBar)] IUIText playerHealthPercentageText,
			[KeyFilter(UnityUIRegisterationKey.PlayerHealthBar)] IUIFillableImage playerHealthBar,
			IReadonlyLocalPlayerDetails playerDetails,
			IEntityDataChangeCallbackRegisterable entityDataCallbackRegister)
		{
			PlayerHealthPercentageText = playerHealthPercentageText ?? throw new ArgumentNullException(nameof(playerHealthPercentageText));
			PlayerHealthBar = playerHealthBar ?? throw new ArgumentNullException(nameof(playerHealthBar));
			PlayerDetails = playerDetails ?? throw new ArgumentNullException(nameof(playerDetails));
			EntityDataCallbackRegister = entityDataCallbackRegister ?? throw new ArgumentNullException(nameof(entityDataCallbackRegister));
		}

		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			//Join main thread, we must access the UI.
			await new UnityYieldAwaitable();

			RecalulateHealthUI(PlayerDetails.EntityData.GetFieldValue<int>(EntityDataFieldType.EntityCurrentHealth));

			//TODO: Any way to do type inference on generics for Action types???
			//We should subscribe to changes in the current health
			EntityDataCallbackRegister.RegisterCallback<int>(PlayerDetails.LocalPlayerGuid, EntityDataFieldType.EntityCurrentHealth, OnCurrentHealthChangedValue);
		}

		private void RecalulateHealthUI(int currentHealth)
		{
			float healthPercentage = (float)currentHealth / PlayerDetails.EntityData.GetFieldValue<int>(EntityDataFieldType.EntityMaxHealth);

			PlayerHealthBar.FillAmount = healthPercentage;

			//Also we want to see the percentage text
			PlayerHealthPercentageText.Text = $"{(int)(healthPercentage * 100.0f)}%";
		}

		private void OnCurrentHealthChangedValue(NetworkEntityGuid source, EntityDataChangedArgs<int> changeArgs)
		{
			RecalulateHealthUI(changeArgs.NewValue);
		}
	}
}
