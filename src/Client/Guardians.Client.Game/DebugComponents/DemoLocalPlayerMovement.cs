using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class DemoLocalPlayerMovement : MonoBehaviour
	{
		[SerializeField]
		private float Speed = 3;

		void Update()
		{
			var x = Input.GetAxis("Horizontal") * Time.deltaTime * Speed;
			var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;

			this.transform.position += new Vector3(x, 0, z);
		}
	}
}
