using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using UnityEngine.UI;

namespace Guardians
{
	public sealed class PlayerUnitFrameController : IGameInitializable
	{
		private IUIText PlayerNameTextField { get; }

		/// <inheritdoc />
		public PlayerUnitFrameController([KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIText playerNameTextField)
		{
			PlayerNameTextField = playerNameTextField ?? throw new ArgumentNullException(nameof(playerNameTextField));
		}

		public async Task OnGameInitialized()
		{
			//Join main thread to touch Unity3D UI
			await new UnityYieldAwaitable();

			PlayerNameTextField.Text = "Testing New IoC";
		}
	}
}
