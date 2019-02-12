using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PSOBB
{
	[Injectee]
	public sealed class DebugChangeLocalPlayerModel : NetworkRequestSender
	{
		[SerializeField]
		private int ModelId;

		[SerializeField]
		private InputField ModelChangeInputField;

		[Button]
		public void SendModelChangeRequest()
		{
			if(ModelChangeInputField != null)
				ModelId = int.Parse(ModelChangeInputField.text);

			this.SendService.SendMessage(new PlayerModelChangeRequestPayload(ModelId));
		}
	}
}
