using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Guardians.Client.Game.Initializables
{

	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene)]
	public sealed class SocialConnectionGameInitializable : IGameInitializable
	{
		private HubConnection SocialConnection { get; }

		/// <inheritdoc />
		public SocialConnectionGameInitializable([NotNull] HubConnection socialConnection)
		{
			SocialConnection = socialConnection ?? throw new ArgumentNullException(nameof(socialConnection));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			//Just start the service when the game initializes
			//This will make it so that the signalR clients will start to recieve messages.
			return SocialConnection.StartAsync();
		}
	}
}
