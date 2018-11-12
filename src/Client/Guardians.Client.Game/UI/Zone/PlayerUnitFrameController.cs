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

		private IReadonlyLocalPlayerDetails LocalPlayerDetails { get; }

		private INameQueryService NameQueryable { get; }

		/// <inheritdoc />
		public PlayerUnitFrameController(
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIText playerNameTextField,
			IReadonlyLocalPlayerDetails localPlayerDetails,
			INameQueryService nameQueryable)
		{
			PlayerNameTextField = playerNameTextField ?? throw new ArgumentNullException(nameof(playerNameTextField));
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
		}

		public async Task OnGameInitialized()
		{
			string nameQueryResponseValue = await NameQueryable.RetrieveAsync(LocalPlayerDetails.LocalPlayerGuid.EntityId)
				.ConfigureAwait(false);

			//Join main thread to touch Unity3D UI
			await new UnityYieldAwaitable();

			PlayerNameTextField.Text = nameQueryResponseValue;
		}
	}
}
