using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GladMMO
{
	public sealed class AvatarLerper : MonoBehaviour
	{
		private Vector3 lastPosition;

		[SerializeField]
		private Transform TransformToFollow;

		[Range(0.0f, 1.0f)]
		[SerializeField]
		private float LerpPower = 0.7f;

		void Start()
		{
			lastPosition = transform.position;
		}

		void LateUpdate()
		{
			transform.position = Vector3.Lerp(lastPosition, TransformToFollow.position, LerpPower);
			lastPosition = transform.position;
		}
	}
}
