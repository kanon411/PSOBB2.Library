using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Guardians
{
	[Injectee]
	public sealed class DebugSwitchZoneServersComponent : NetworkRequestSender
	{
		[SerializeField]
		private InputField Input;

		public void SwitchZoneInstances()
		{
			//Just load
			int zoneId = Int32.Parse(Input.text);

			//TODO: Create session on the zone for debug/demo purposes
			//We just load to the loading screen and we'll reload the into the current zone
			SceneManager.LoadSceneAsync((int)GameInitializableSceneSpecificationAttribute.SceneType.WorldDownloadingScreen, LoadSceneMode.Single).allowSceneActivation = true;
		}
	}
}
