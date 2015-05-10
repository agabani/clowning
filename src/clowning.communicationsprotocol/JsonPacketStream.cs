using System;
using System.Collections.Generic;
using System.IO;

namespace clowning.communicationsprotocol
{
    public class JsonPacketStream
    {
        private readonly byte[] _streamBuffer = new byte[4096];
        private MemoryStream _memoryStream;

        public JsonPacketStream()
        {
            _memoryStream = new MemoryStream(_streamBuffer);
        }

        public ICollection<byte[]> FeedBytes(byte[] feed)
        {
            List<byte[]> result = null;

            _memoryStream.Write(feed, 0, feed.Length);

            bool finished;
            do
            {
                finished = true;

                if (_memoryStream.Position >= 5)
                {
                    var contentLength = BitConverter.ToInt32(_streamBuffer, 1);
                    var packetLength = 5 + contentLength;

                    if (_memoryStream.Position >= packetLength)
                    {
                        var streamLength = _memoryStream.Position;
                        byte[] buffer = new byte[packetLength];

                        _memoryStream.Position = 0;
                        _memoryStream.Read(buffer, 0, packetLength);

                        for (var i = 0; i < streamLength - packetLength; i++)
                        {
                            _streamBuffer[i] = _streamBuffer[i + packetLength];
                        }

                        _memoryStream.Position = streamLength - packetLength;

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
    }
}