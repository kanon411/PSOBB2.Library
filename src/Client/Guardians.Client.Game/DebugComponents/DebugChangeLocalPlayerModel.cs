using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class DebugChangeLocalPlayerModel : NetworkRequestSender
	{
		[SerializeField]
		private int ModelId;

		[Button]
		public void SendModelChangeRequest()
		{
			this.SendService.SendMessage(new PlayerModelChangeRequestPayload(ModelId));
		}
	}
}
