using System;
using System.Collections.Generic;
using System.Text;

namespace Dissonance
{
	public static class CustomProjectDissonanceAsserts
	{
		/// <summary>
		/// Throws if the audio data in the voice packet is shared and requires copying.
		/// Nothing happens if the audio data has already been copied and isn't a shared buffer.
		/// </summary>
		public static void AssertVoicePacketAudioDataAlreadyCopied()
		{

		}

		public static void AssertVoiceDuckingIsntNeeded()
		{

		}
	}
}
