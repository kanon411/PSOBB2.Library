using System;
using System.Collections.Generic;
using System.Text;
using Dissonance.VAD;

namespace Dissonance
{
	//Custom Change: Adding an interface for voice activision subscription.
	public interface IVoiceActivisionSubscribable
	{
		/// <summary>
		///     Subscribes to automatic voice detection.
		/// </summary>
		/// <param name="listener">
		///     The listener which is to receive notification when the player starts and stops speaking via
		///     automatic voice detection.
		/// </param>
		void SubcribeToVoiceActivation([NotNull] IVoiceActivationListener listener);

		/// <summary>
		///     Unsubsribes from automatic voice detection.
		/// </summary>
		/// <param name="listener"></param>
		void UnsubscribeFromVoiceActivation([NotNull] IVoiceActivationListener listener);
	}
}
