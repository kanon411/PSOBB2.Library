namespace Dissonance.Audio.Playback
{
    public struct PlaybackOptions
    {
        private readonly bool _isPositional;
        private readonly float _amplitudeMultiplier;

        public PlaybackOptions(bool isPositional, float amplitudeMultiplier)
        {
            _isPositional = isPositional;
            _amplitudeMultiplier = amplitudeMultiplier;
        }

        /// <summary>
        /// Get if audio on this channel is positional
        /// </summary>
        public bool IsPositional { get { return _isPositional; } }

        /// <summary>
        /// Get the amplitude multiplier applied to audio played through this channel
        /// </summary>
        public float AmplitudeMultiplier { get { return _amplitudeMultiplier; } }
    }
}
