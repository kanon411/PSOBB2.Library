using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public sealed class DemoSettableTrackers : MonoBehaviour
	{
		[SerializeField]
		private Transform _CameraTrackerTransform;

		/// <summary>
		/// The camera tracker transform.
		/// </summary>
		public Transform CameraTrackerTransform => _CameraTrackerTransform;

		[SerializeField]
		private Transform _LeftHandTrackerTransform;

		/// <summary>
		/// The camera tracker transform.
		/// </summary>
		public Transform LeftHandTrackerTransform => _LeftHandTrackerTransform;

		[SerializeField]
		private Transform _RightHandTrackerTransform;

		/// <summary>
		/// The camera tracker transform.
		/// </summary>
		public Transform RightHandTrackerTransform => _RightHandTrackerTransform;
	}
}
