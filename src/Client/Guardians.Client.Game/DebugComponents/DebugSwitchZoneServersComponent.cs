﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using Nito.AsyncEx;
using SceneJect.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Guardians
{
	[Injectee]
	public sealed class DebugSwitchZoneServersComponent : MonoBehaviour
	{
		[SerializeField]
		private InputField Input;

		[Inject]
		private IConnectionService ConnectionService { get; set; }

		//We need this for session management
		[Inject]
		private ICharacterService CharacterService { get; set; }

		[Inject]
		private ICharacterDataRepository DataRepo { get; set; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; set; }

		public void SwitchZoneInstances()
		{
			//Just load
			int zoneId = Int32.Parse(Input.text);

			//Do not use AsyncContext, it will deadlock
			TaskEx.Run(async () =>
			{
				await ConnectionService.DisconnectAsync(0)
					.ConfigureAwait(false);

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

				//We just load to the loading screen and we'll reload the into the current zone
				SceneManager.LoadSceneAsync((int)GameInitializableSceneSpecificationAttribute.SceneType.WorldDownloadingScreen, LoadSceneMode.Single).allowSceneActivation = true;
			});

			//We can't await to unwrap exceptions because
			//the other thing joins the unity3d main thread via synccontext hack
			//and therefore will deadlock
		}
	}
}