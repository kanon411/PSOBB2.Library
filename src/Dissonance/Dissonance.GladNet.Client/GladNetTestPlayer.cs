using System;
using System.Collections.Generic;
using System.Text;
using Guardians;
using SceneJect.Common;
using UnityEngine;

namespace Dissonance
{
	/// <summary>
	/// Component that represents a the location/rotation of a player.
	/// </summary>
	[Injectee]
	public sealed class GladNetTestPlayer : MonoBehaviour, IDissonancePlayer
	{
		[Inject]
		private NetworkEntityGuid EntityGuid { get; set; }

		[SerializeField]
		private NetworkPlayerType PlayerType;

		[SerializeField]
		private Transform AudioLocationTransform;

		private Lazy<string> ComputedName { get; }

		public GladNetTestPlayer()
		{
			ComputedName = new Lazy<string>(() => ComputeName());
		}

		public string ComputeName()
		{
			return EntityGuid.RawGuidValue.ToString();
		}

		//To prevent GC waste we just initialize this once.
		/// <inheritdoc />
		public string PlayerId => ComputedName.Value;

		/// <inheritdoc />
		public Vector3 Position => AudioLocationTransform.position;

		/// <inheritdoc />
		public Quaternion Rotation => AudioLocationTransform.rotation;

		//TODO: is it ok if we don't change this?
		/// <inheritdoc />
		public NetworkPlayerType Type => PlayerType;

		/// <inheritdoc />
		public bool IsTracking => true;
	}
}
