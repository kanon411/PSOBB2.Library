using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.PreZoneBurstingScreen)]
	public class PreBurstSceneChangeChangerEventListener : BaseSingleEventListenerInitializable<IServerRequestedSceneChangeEventSubscribable, ServerRequestedSceneChangeEventArgs>
	{
		/// <inheritdoc />
		public PreBurstSceneChangeChangerEventListener(IServerRequestedSceneChangeEventSubscribable subscriptionService) : base(subscriptionService)
		{
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, ServerRequestedSceneChangeEventArgs args)
		{
			//TODO: Is there a better way to abstract this?
			//We are not likely on the main thread at this point.
			//So we need to queue up a scene change on the main thread

			UnityExtended.UnityMainThreadContext.Send(state =>
			{
				//TODO: We should handle REAL scene changes here.
				int currentIndex = SceneManager.GetActiveScene().buildIndex;

				//PreBurst scene should always be the last scene before playable scenes.
				int sceneIndexToLoad = currentIndex + (int)args.SceneRequested;

				SceneManager.LoadSceneAsync(sceneIndexToLoad, LoadSceneMode.Single).allowSceneActivation = true;
			}, null);
		}
	}
}
