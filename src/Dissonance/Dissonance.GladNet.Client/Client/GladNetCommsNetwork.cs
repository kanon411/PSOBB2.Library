using System;
using System.Collections.Generic;
using System.Text;
using Dissonance.Networking;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace Dissonance.GladNet
{
	[Injectee]
	public sealed class GladNetCommsNetwork : GladNetBaseCommsNetwork<GladNetDissonanceClient, long, Unit>
	{
		[Inject]
		private Func<GladNetDissonanceClient> ClientFactory { get; set; }

		[Inject]
		private IConnectionService ConnectionService { get; set; }

		[Inject]
		private GladNetDissonanceClientCurrentInstanceAdapter ConnectionInstanceProvider { get; set; }

		protected override GladNetDissonanceClient CreateClient([Dissonance.CanBeNull] Unit connectionParameters)
		{
			Debug.Log($"Creating Dissonance.GladNet client.");
			return ConnectionInstanceProvider.ClientInstance = ClientFactory();
		}

		protected override void Update()
		{
			base.Update();

			//TODO: We should also check the entity session state.
			if(IsInitialized && ConnectionService.isConnected)
			{
				if(Mode != NetworkMode.Client)
					RunAsClient(Unit.None);
			}
		}
	}
}
