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

		void Update()
		{
			transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0.0f) * LookSpeed;
		}
	}
}
