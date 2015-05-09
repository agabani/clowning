using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Animation
{
    [TestFixture]
    public class Unflash
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_unflash_light()
        {
            Given_that_I_have_a_client();
            When_I_unflash_the_device();
            Then_the_device_should_not_be_flashing();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_unflash_invalid_device(int deviceNumber)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_unflash(deviceNumber));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_unflash_the_device()
        {
            _successful = true;

            for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
            {
                _successful &= _blyncClient.SetFlash(device, false);
            }
        }

        private TestDelegate When_I_unflash(int deviceNumber)
        {
            return () => _blyncClient.SetFlash(deviceNumber, false);
        }

        private void Then_the_device_should_not_be_flashing()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}