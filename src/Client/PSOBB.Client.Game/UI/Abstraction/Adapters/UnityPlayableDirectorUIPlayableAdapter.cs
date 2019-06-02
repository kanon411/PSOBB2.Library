using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSOBB;
using UnityEngine.Playables;

namespace PSOBB
{
	public sealed class UnityPlayableDirectorUIPlayableAdapter : BaseUnityUIAdapter<PlayableDirector, IUIPlayable>, IUIPlayable
	{
		/// <inheritdoc />
		public bool isPlaying => this.UnityUIObject.state == PlayState.Playing;

		/// <inheritdoc />
		public void Play()
		{
			this.UnityUIObject.Play();
		}

		/// <inheritdoc />
		public void Stop()
		{
			this.UnityUIObject.Stop();
		}
	}
}
