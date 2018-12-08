using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dissonance.Audio.Playback;

namespace Dissonance.Networking
{
	/// <summary>
	/// A packet of encoded voice data
	/// </summary>
	public struct VoicePacket
    {
        /// <summary>
        /// ID of the player who sent this voice packet
        /// </summary>
        public readonly string SenderPlayerId;

        /// <summary>
        /// The encoded audio to pass directly to the codec
        /// </summary>
        public readonly ArraySegment<byte> EncodedAudioFrame;

        /// <summary>
        /// The (wrapping) sequence number of this packet
        /// </summary>
        public readonly uint SequenceNumber;

        private readonly PlaybackOptions _options;
        public PlaybackOptions PlaybackOptions
        {
            get { return _options; }
        }

        /// <summary>
        /// Create a new voice packet
        /// </summary>
        /// <param name="senderPlayerId"></param>
        /// <param name="priority"></param>
        /// <param name="ampMul"></param>
        /// <param name="positional"></param>
        /// <param name="encodedAudioFrame">The encoded audio data. The data will be copied out of this array as soon as it is passed to the decoder
        /// pipeline (i.e. you can re-use this array right away)</param>
        /// <param name="sequence"></param>
        /// <param name="channels">List of all channels this voice packet is being spoken on. Data will be copied out of the list as soon as it is
        /// passed to the decoder pipeline (i.e. you can re-use this array right away)</param>
        public VoicePacket(string senderPlayerId, float ampMul, bool positional, ArraySegment<byte> encodedAudioFrame, uint sequence)
        {
            _options = new PlaybackOptions(positional, ampMul);

            SenderPlayerId = senderPlayerId;
            EncodedAudioFrame = encodedAudioFrame;
            SequenceNumber = sequence;
        }
    }

    /// <summary>
    /// A player entered or exited a room
    /// </summary>
    public struct RoomEvent
    {
        /// <summary>
        /// ID of the player who sent this message
        /// </summary>
        public string PlayerName;

        /// <summary>
        /// Name of the room they entered/exited
        /// </summary>
        public string Room;

        /// <summary>
        /// Whether they joined or left the room
        /// </summary>
        public bool Joined;

        /// <summary>
        /// All the rooms this client is in
        /// </summary>
        internal ReadOnlyCollection<string> Rooms;

        internal RoomEvent([NotNull] string name, [NotNull] string room, bool joined, [NotNull] ReadOnlyCollection<string> rooms)
        {
            PlayerName = name;
            Room = room;
            Joined = joined;
            Rooms = rooms;
        }
    }

    /// <summary>
    /// The mode of the network
    /// </summary>
    public enum NetworkMode
    {
        /// <summary>
        /// No network is established
        /// </summary>
        None,

        /// <summary>
        /// Local machine is hosting the session (both a client and a server)
        /// </summary>
        Host,

        /// <summary>
        /// Local machine is purely a client
        /// </summary>
        Client,

        /// <summary>
        /// Local machine is purely a server
        /// </summary>
        DedicatedServer
    }

    /// <summary>
    /// Status of the connection to the session server
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Not connected
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connected/Connecting, but not fully capable of tranmitting voice
        /// </summary>
        Degraded,

        /// <summary>
        /// Connected to the server
        /// </summary>
        Connected
    }

    public static class NetworkModeExtensions
    {
        public static bool IsServerEnabled(this NetworkMode mode)
        {
            switch (mode)
            {
                case NetworkMode.Host:
                case NetworkMode.DedicatedServer:
                    return true;
                case NetworkMode.None:
                case NetworkMode.Client:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
        }

        public static bool IsClientEnabled(this NetworkMode mode)
        {
            switch (mode)
            {
                case NetworkMode.Host:
                case NetworkMode.Client:
                    return true;
                case NetworkMode.None:
                case NetworkMode.DedicatedServer:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
        }
    }

    public interface ICommsNetwork
    {
        /// <summary>
        /// Gets the network connection status.
        /// </summary>
        ConnectionStatus Status { get; }

        /// <summary>
        ///     Attempts a connection to the voice server.
        /// </summary>
        /// <param name="playerName">The name of the local player. Must be unique on the network.</param>
        /// <param name="rooms">The room membership collection the network should track.</param>
        /// <param name="playerChannels">The player channels collection the network should track.</param>
        /// <param name="roomChannels">The room channels collection the network should track.</param>
        /// <param name="codecSettings">The audio codec being used on the network.</param>
        void Initialize(string playerName, Rooms rooms, CodecSettings codecSettings);

        /// <summary>
        /// Event which is raised when the network mode changes.
        /// </summary>
        event Action<NetworkMode> ModeChanged;

        /// <summary>
        /// Get the current network mode
        /// </summary>
        NetworkMode Mode { get; }

        /// <summary>
        /// Event which is raised when a remote player joins the Dissonance session. Passed the unique ID of the player
        /// </summary>
        event Action<string, CodecSettings> PlayerJoined;

        /// <summary>
        /// Event which is raised when a remote player leaves the Dissonance session. Passed the unique ID of the player
        /// </summary>
        event Action<string> PlayerLeft;

        /// <summary>
        /// Event which is raised when a voice packet is received
        /// </summary>
        event Action<VoicePacket> VoicePacketReceived;

        /// <summary>
        /// Event which is raised when a remote player begins speaking. Passed the unique ID of the player
        /// </summary>
        event Action<string> PlayerStartedSpeaking;

        /// <summary>
        /// Event which is raised when a remote player stops speaking. Passed the unique ID of the player
        /// </summary>
        event Action<string> PlayerStoppedSpeaking;

        /// <summary>
        /// Event which is raised when a remote player enters a room.
        /// </summary>
        event Action<RoomEvent> PlayerEnteredRoom;

        /// <summary>
        /// Event which is raised when a remote player exits a room.
        /// </summary>
        event Action<RoomEvent> PlayerExitedRoom;

        /// <summary>
        /// Send the given voice data to the specified recipients.
        /// </summary>
        /// <remarks>The implementation of this method MUST NOT keep a reference to the given array beyond the scope of this method (the array is recycled for other uses)</remarks>
        /// <param name="data">The encoded audio data to send.</param>
        void SendVoice(ArraySegment<byte> data);
    }
}
