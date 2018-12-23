using System;
using System.Collections.Generic;
using System.Text;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class DemoNetworkTransform : NetworkRequestSender
	{
		private Vector3 LastPosition;

		private Vector2 LastDirection;

		private Quaternion LastCameraRotation;

		[SerializeField]
		private Transform CameraTransform;

		void Start()
		{
			//This is kinda a hack, to sidestep some
			//issues encountered with Unity2018 prefab reference serialization
			if(CameraTransform == null)
				CameraTransform = Camera.main.transform;

			LastCameraRotation = CameraTransform.rotation;
			LastPosition = transform.position;
			LastDirection = Vector2.zero;
		}

		void Update()
		{
			Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			//If we aren't moving don't send
			if(LastPosition == transform.position && LastDirection == direction && CameraTransform.rotation == LastCameraRotation)
				return;

			//TODO: Send timestamp
			//This is just demo code to network the position for the demo.
			this.SendService.SendMessage(new ClientMovementDataUpdateRequest(new PositionChangeMovementDataWithLook(0, this.transform.position, direction, CameraTransform.localEulerAngles, transform.eulerAngles.y)));

			LastPosition = transform.position;
			LastDirection = direction;
			LastCameraRotation = CameraTransform.rotation;
		}
	}
}
