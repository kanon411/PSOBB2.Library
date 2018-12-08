using System;
using System.Collections.Generic;

namespace Dissonance.Networking.Client
{
    public interface IClient
    {
        void SendReliable(ArraySegment<byte> arraySegment);

        void SendUnreliable(ArraySegment<byte> arraySegment);
    }
}
