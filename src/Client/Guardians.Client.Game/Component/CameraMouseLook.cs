using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class CameraMouseLook : MonoBehaviour
	{
		[SerializeField]
		private float LookSpeed = 3;

		[SerializeField]
		private Vector2 MaxLookAngle = new Vector2(60.0f, 60.0f);

		[SerializeField]
		private GameObject RootRotationalObject;

		private Vector3 CurrentRotation;

		void Start()
		{
			CurrentRotation = transform.localEulerAngles;
		}

		void Update()
		{
			//Mathf.Clamp(Input.GetAxis("Mouse X") * LookSpeed + CurrentRotation.y, -MaxLookAngle.y, MaxLookAngle.y);

			//Additive rotation around the y-axis for the IK root.
			float rotationalMovement = Input.GetAxis("Mouse X") * LookSpeed;

			//TODO: Kinda slow
			CurrentRotation = new Vector3(Mathf.Clamp(-Input.GetAxis("Mouse Y") * LookSpeed + CurrentRotation.x, -MaxLookAngle.x, MaxLookAngle.x), 0, 0);

			transform.localEulerAngles = CurrentRotation;
			RootRotationalObject.transform.Rotate(Vector3.up, rotationalMovement);
		}
	}
}
