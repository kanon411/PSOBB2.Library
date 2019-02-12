using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using UnityEngine;
using UnityEngine.UI;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.ZoneGameScene)]
	public sealed class PlayerUnitFrameController : IGameInitializable
	{
		private IUIText PlayerNameTextField { get; }

		private IReadonlyLocalPlayerDetails LocalPlayerDetails { get; }

		private INameQueryService NameQueryable { get; }

		private IUIImage PlayerAvatarPortriat { get; }

		/// <inheritdoc />
		public PlayerUnitFrameController(
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIText playerNameTextField,
			IReadonlyLocalPlayerDetails localPlayerDetails,
			INameQueryService nameQueryable,
			[KeyFilter(UnityUIRegisterationKey.PlayerUnitFrame)] IUIImage playerAvatarPortriat)
		{
			PlayerNameTextField = playerNameTextField ?? throw new ArgumentNullException(nameof(playerNameTextField));
			LocalPlayerDetails = localPlayerDetails ?? throw new ArgumentNullException(nameof(localPlayerDetails));
			NameQueryable = nameQueryable ?? throw new ArgumentNullException(nameof(nameQueryable));
			PlayerAvatarPortriat = playerAvatarPortriat ?? throw new ArgumentNullException(nameof(playerAvatarPortriat));
		}

		public async Task OnGameInitialized()
		{
			string nameQueryResponseValue = await NameQueryable.RetrieveAsync(LocalPlayerDetails.LocalPlayerGuid.EntityId)
				.ConfigureAwait(false);

			//Join main thread to touch Unity3D UI
			await new UnityYieldAwaitable();

			PlayerNameTextField.Text = nameQueryResponseValue;

			//TODO: We should not directly query Gaia here. We should abstract this into async dictionary we can query for loaded avatars
		}
	}
}
