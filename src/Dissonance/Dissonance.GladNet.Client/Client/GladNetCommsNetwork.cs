using System;
using System.Collections.Generic;
using System.Text;
using Dissonance.Networking;
using UnityEngine;

namespace Dissonance.GladNet
{
	public sealed class GladNetCommsNetwork : GladNetBaseCommsNetwork<GladNetDissonanceClient, long, Unit>
	{
		protected override GladNetDissonanceClient CreateClient([Dissonance.CanBeNull] Unit connectionParameters)
		{
			Debug.Log($"Creating Dissonance.GladNet client.");
			return new GladNetDissonanceClient(this);
		}

		protected override void Update()
		{
			base.Update();

			if(IsInitialized)
			{
				if(Mode != NetworkMode.Client)
					RunAsClient(Unit.None);
			}
		}
	}
}
