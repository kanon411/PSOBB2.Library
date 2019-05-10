using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GladMMO
{
	public interface IServerRequestedSceneChangeEventSubscribable
	{
		event EventHandler<ServerRequestedSceneChangeEventArgs> OnServerRequestedSceneChange;
	}

	public sealed class ServerRequestedSceneChangeEventArgs : EventArgs
	{
		public PlayableGameScene SceneRequested { get; }

		/// <inheritdoc />
		public ServerRequestedSceneChangeEventArgs(PlayableGameScene sceneRequested)
		{
			if(!Enum.IsDefined(typeof(PlayableGameScene), sceneRequested)) throw new InvalidEnumArgumentException(nameof(sceneRequested), (int)sceneRequested, typeof(PlayableGameScene));

			SceneRequested = sceneRequested;
		}
	}
}
