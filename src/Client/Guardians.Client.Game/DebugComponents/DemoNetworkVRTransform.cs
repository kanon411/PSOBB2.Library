using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	//TODO: This is just for demo purposes
	[Injectee]
	public sealed class DemoNetworkVRTransform : NetworkRequestSender
	{
		[SerializeField]
		private DemoSettableTrackers Trackers;

		void Update()
		{
			Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			//If we aren't moving don't send
			//if(LastPosition == transform.position && LastDirection == direction && CameraTransform.rotation == LastCameraRotation)
			//	return;

			//TODO: Send timestamp
			//This is just demo code to network the position for the demo.
			this.SendService.SendMessage(new ClientMovementDataUpdateRequest(new PositionChangeMovementDataDefaultVR(0, this.transform.position, direction, 
				new GameObjectEntitySerializableTransform(Trackers.CameraTrackerTransform),
				new GameObjectEntitySerializableTransform(Trackers.LeftHandTrackerTransform),
				new GameObjectEntitySerializableTransform(Trackers.RightHandTrackerTransform))));
		}
	}
}
