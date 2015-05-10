using System;
using System.Collections.Generic;
using System.IO;
using clowning.communicationsprotocol.Stream;

namespace clowning.communicationsprotocol.Json.Stream
{
    public class JsonPacketStream : IPacketStream
    {
        private const int HeaderLength = 5;
        private readonly byte[] _streamBuffer;
        private readonly MemoryStream _memoryStream;

        public JsonPacketStream()
        {
            _streamBuffer = new byte[4096];
            _memoryStream = new MemoryStream(_streamBuffer);
        }

        public JsonPacketStream(int bufferSize)
        {
            _streamBuffer = new byte[bufferSize];
            _memoryStream = new MemoryStream(_streamBuffer);
        }

        public ICollection<byte[]> ParseBytes(byte[] feed)
        {
            List<byte[]> result = null;

            _memoryStream.Write(feed, 0, feed.Length);

            bool finished;
            do
            {
                finished = true;

                if (_memoryStream.Position >= HeaderLength)
                {
                    var contentLength = BitConverter.ToInt32(_streamBuffer, 1);
                    var packetLength = HeaderLength + contentLength;

                    if (_memoryStream.Position >= packetLength)
                    {
                        var streamLength = _memoryStream.Position;
                        byte[] buffer = new byte[packetLength];

                        _memoryStream.Position = 0;
                        _memoryStream.Read(buffer, 0, packetLength);

                        LeftShiftStream(streamLength, packetLength);

                        if (result == null)
                        {
                            result = new List<byte[]>();
                        }

                        result.Add(buffer);
                        finished = false;
                    }
                }
            } while (finished == false);

            return result;
        }

        private void LeftShiftStream(long streamLength, long packetLength)
        {
            for (var i = 0; i < streamLength - packetLength; i++)
            {
                _streamBuffer[i] = _streamBuffer[i + packetLength];
            }

            _memoryStream.Position = streamLength - packetLength;
        }

        public void Dispose()
        {
            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
            }
        }
    }
}