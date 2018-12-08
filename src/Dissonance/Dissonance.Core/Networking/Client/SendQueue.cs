using System;
using System.Collections.Generic;
using Dissonance.Datastructures;

namespace Dissonance.Networking.Client
{
    /// <inheritdoc />
    internal class SendQueue<TPeer>
        : ISendQueue<TPeer>
        where TPeer : struct
    {
        #region fields and properties
        private static readonly Log Log = Logs.Create(LogCategory.Network, typeof(SendQueue<TPeer>).Name);

        private readonly IClient _client;

        private readonly List<ArraySegment<byte>> _serverReliableQueue = new List<ArraySegment<byte>>();
        private readonly List<ArraySegment<byte>> _serverUnreliableQueue = new List<ArraySegment<byte>>();

        private readonly ConcurrentPool<byte[]> _sendBufferPool;
        public ConcurrentPool<byte[]> SendBufferPool
        {
            get { return _sendBufferPool; }
        }

        private readonly Pool<List<ClientInfo<TPeer?>>> _listPool = new Pool<List<ClientInfo<TPeer?>>>(32, () => new List<ClientInfo<TPeer?>>());
        #endregion

        #region constructor
        public SendQueue([NotNull] IClient client, [NotNull] ConcurrentPool<byte[]> bytePool)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (bytePool == null) throw new ArgumentNullException("bytePool");

            _client = client;
            _sendBufferPool = bytePool;
        }
        #endregion

        public void Update()
        {
            //Reliable traffic to server
            for (var i = 0; i < _serverReliableQueue.Count; i++)
            {
                var item = _serverReliableQueue[i];
                _client.SendReliable(item);

                // ReSharper disable once AssignNullToNotNullAttribute (Justification: Array segment cannot be null)
                Recycle(item.Array);
            }
            _serverReliableQueue.Clear();

            //Unreliable traffic to server
            for (var i = 0; i < _serverUnreliableQueue.Count; i++)
            {
                var item = _serverUnreliableQueue[i];
                _client.SendUnreliable(item);

                // ReSharper disable once AssignNullToNotNullAttribute (Justification: Array segment cannot be null)
                Recycle(item.Array);
            }
            _serverUnreliableQueue.Clear();
        }

        private void Recycle([NotNull] byte[] array)
        {
            if (array == null) throw new ArgumentNullException("array");

            _sendBufferPool.Put(array);
        }

        public void Stop()
        {
            var dropped = _serverReliableQueue.Count
                        + _serverUnreliableQueue.Count;

            Log.Debug("Stopped network SendQueue (dropping {0} remaining packets)", dropped);

            _serverReliableQueue.Clear();
            _serverUnreliableQueue.Clear();
        }

        #region Enqueue
        public void EnqueueReliable(ArraySegment<byte> packet)
        {
            if (packet.Array == null) throw new ArgumentNullException("packet");

            _serverReliableQueue.Add(packet);
        }

        public void EnqeueUnreliable(ArraySegment<byte> packet)
        {
            if (packet.Array == null) throw new ArgumentNullException("packet");

            _serverUnreliableQueue.Add(packet);
        }
        #endregion
    }

    internal interface ISendQueue<TPeer>
        where TPeer : struct
    {
        [NotNull] ConcurrentPool<byte[]> SendBufferPool { get; }

        /// <summary>
        /// Send a reliable message to the server
        /// </summary>
        void EnqueueReliable(ArraySegment<byte> packet);

        /// <summary>
        /// Send an unreliable message to the server
        /// </summary>
        void EnqeueUnreliable(ArraySegment<byte> packet);
    }
}
