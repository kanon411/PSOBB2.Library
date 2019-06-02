using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using Nito.AsyncEx;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unitysync.Async;

namespace PSOBB
{
	[Injectee]
	public sealed class DebugSwitchZoneServersComponent : MonoBehaviour
	{
		[SerializeField]
		private InputField Input;

		[ShowInInspector]
		[ReadOnly]
		private bool wasFired = false;

		[Inject]
		private IConnectionService ConnectionService { get; set; }

		//We need this for session management
		[Inject]
		private ICharacterService CharacterService { get; set; }

		[Inject]
		private ICharacterDataRepository DataRepo { get; set; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; set; }

		[Inject]
		private IEnumerable<IDisposable> Disposables { get; set; }

		public void SwitchZoneInstances()
		{
			//Make it so that we can only fire 1 instance change
			if(wasFired)
				return;

			wasFired = true;

			//Just load
			int zoneId = Int32.Parse(Input.text);

			ConnectionService.DisconnectAsync(0)
				.UnityAsyncContinueWith(this, () => OnFinishedDisconnecting(zoneId))
				.ConfigureAwait(false);

			//We can't await to unwrap exceptions because
			//the other thing joins the unity3d main thread via synccontext hack
			//and therefore will deadlock
		}

		private async Task OnFinishedDisconnecting(int zoneId)
		{
			//Now we need to try to create a new session
			while(true)
			{
				CharacterSessionEnterResponse enterResponse = await CharacterService.SetCharacterSessionData(DataRepo.CharacterId, zoneId, AuthTokenRepo.RetrieveWithType())
					.ConfigureAwait(false);

				//While the response isn't successful we shouild log way, and continue if it's
				if(enterResponse.isSuccessful)
					break;

				await Task.Delay(500)
					.ConfigureAwait(false);

				Debug.LogError($"Failed to set character session data to ZoneId: {zoneId} for Reason: {enterResponse.ResultCode}");
			}

			//Join main unity thread
			await new UnityYieldAwaitable();

			//We should dispose any resources in the scene that are disposable before loading a new scene
			foreach(var d in Disposables)
				d.Dispose();

			//We just load to the loading screen and we'll reload the into the current zone
			SceneManager.LoadSceneAsync((int)GameSceneType.WorldDownloadingScreen, LoadSceneMode.Single).allowSceneActivation = true;
		}
	}
}
