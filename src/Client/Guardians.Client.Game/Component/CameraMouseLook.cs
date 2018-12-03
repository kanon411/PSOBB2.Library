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

		private Vector3 CurrentRotation;

		void Start()
		{
			CurrentRotation = transform.eulerAngles;
		}

		void Update()
		{
			//TODO: Kinda slow
			CurrentRotation = new Vector3(Mathf.Clamp(-Input.GetAxis("Mouse Y") * LookSpeed + CurrentRotation.x, -60.0f, 60.0f), Mathf.Clamp(Input.GetAxis("Mouse X") * LookSpeed + CurrentRotation.y, -60.0f, 60.0f), 0);

			transform.eulerAngles = CurrentRotation;
		}
	}
}
