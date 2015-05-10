using NUnit.Framework;

namespace clowning.communicationsprotocol.tests
{
    [TestFixture]
    public class Serialization
    {
        [Test]
        public void Should_be_able_to_serialize_and_deserialize_correctly()
        {
            const string expectedMessage = "Hello World!";
            const int expectedType = 4;

            var jsonProtocol = new JsonProtocol();
            var buffer = jsonProtocol.SetPacketContents(expectedType, expectedMessage);

            var actualType = jsonProtocol.GetPacketType(buffer);
            var length = jsonProtocol.GetPacketLength(buffer);
            var actualMessage = jsonProtocol.GetPacketContents(buffer, length);

            Assert.That(actualType, Is.EqualTo(expectedType));
            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }
    }
}
