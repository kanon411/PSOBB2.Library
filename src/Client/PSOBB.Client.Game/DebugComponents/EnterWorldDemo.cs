using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet;
using SceneJect.Common;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace GladMMO
{
	[Injectee]
	public sealed class EnterWorldDemo : NetworkRequestSender
	{
		[SerializeField]
		private InputField Input;

		void Start()
		{
			//TODO: Hack to support VR builds with in-editor non-VR.
			//if(!Application.isEditor)
			//	SendCharacterEnterRequest();
		}

		public void SendCharacterEnterRequest()
		{
			int characterId;

			//TODO: Hack to support VR builds with in-editor non-VR.
			if(Application.isEditor)
				characterId = Int32.Parse(Input.text);
			else
				characterId = new Random().Next();


			//For demo purposes we just request to claim a session with the character ID they entered. We will do setup once we recieve the response.
			this.SendService.SendMessage(new ClientSessionClaimRequestPayload("DEMO", characterId));
		}
	}
}
