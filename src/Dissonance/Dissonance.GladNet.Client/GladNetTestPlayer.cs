using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dissonance
{
	public sealed class GladNetTestPlayer : MonoBehaviour, IDissonancePlayer
	{
		/// <inheritdoc />
		public string PlayerId => "Test";

		/// <inheritdoc />
		public Vector3 Position => this.transform.position;

		/// <inheritdoc />
		public Quaternion Rotation => this.transform.rotation;

		//TODO: is it ok if we don't change this?
		/// <inheritdoc />
		public NetworkPlayerType Type => NetworkPlayerType.Local;

		/// <inheritdoc />
		public bool IsTracking => true;
	}
}
