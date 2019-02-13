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
		public PreBurstSceneChangeChangerEventListener(IServerRequestedSceneChangeEventSubscribable subscriptionService) 
			: base(subscriptionService)
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

				//TODO: Move to a solution that won't require a +1 (will be confusing to designers)
				//PreBurst scene should always be the last scene before playable scenes.
				int sceneIndexToLoad = currentIndex + (int)args.SceneRequested + 1; //otherwise 0th enum index.

				SceneManager.LoadSceneAsync(sceneIndexToLoad, LoadSceneMode.Single).allowSceneActivation = true;
			}, null);
		}
	}
}
