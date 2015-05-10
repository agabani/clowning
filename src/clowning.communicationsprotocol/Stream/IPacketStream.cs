using System;
using System.Collections.Generic;

namespace clowning.communicationsprotocol.Stream
{
    public interface IPacketStream : IDisposable
    {
        ICollection<byte[]> ParseBytes(byte[] feed);
    }
}