using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Stream;
using NUnit.Framework;

namespace clowning.communicationsprotocol.tests
{
    [TestFixture]
    public class Stream
    {
        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Should_be_able_to_stream(int count)
        {
            var jsonProtocol = new JsonProtocol();
            var jsonPacketStream = new JsonPacketStream();

            var messages = new List<byte[]>
            {
                jsonProtocol.SetPacketContents(1, "The quick brown fox jumped over the lazy dog"),
                jsonProtocol.SetPacketContents(2, "Second message to be appended")
            };

            var message = messages[0].Concat(messages[1]).ToArray();

            var messageResult = new List<byte[]>();

            for (int i = 0; i < message.Length; i += count)
            {
                var array = message.Skip(i).Take(count).ToArray();
                var result = jsonPacketStream.ParseBytes(array);

                if (result != null)
                {
                    messageResult.AddRange(result);
                }
            }

            Assert.That(messageResult, Is.EqualTo(messages));
        }
    }
}
